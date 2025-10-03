using FlixTv.Api.Application.Features.Movies.Queries.GetMoviesCount;
using FlixTv.Api.Infrastructure.Seeding.Helpers;
using FlixTv.Api.Infrastructure.Seeding.Options;
using FlixTv.Api.Infrastructure.Seeding.Tmdb;
using FlixTv.Common.Models.RequestModels.Movies;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Infrastructure.Seeding
{
    public sealed class MovieSeeder : IHostedService
    {
        private readonly ILogger<MovieSeeder> _log;
        private readonly IServiceProvider _sp;
        private readonly MoviesSeedingOptions _opt;

        public MovieSeeder(ILogger<MovieSeeder> log, IServiceProvider sp, IOptions<MoviesSeedingOptions> opt)
        {
            _log = log;
            _sp = sp;
            _opt = opt.Value;
        }

        public async Task StartAsync(CancellationToken ct)
        {
            if (!_opt.Enabled) { _log.LogInformation("MovieSeeder disabled."); return; }

            var scopeFactory = _sp.GetRequiredService<IServiceScopeFactory>();

            // TMDB client-ı root-dan rahat götür (scoped deyil, typed HttpClient-dir)
            var tmdb = _sp.GetRequiredService<TmdbClient>();

            // Statistik sorğular üçün qısa async-scope
            await using (var statScope = scopeFactory.CreateAsyncScope())
            {
                var mediatorStat = statScope.ServiceProvider.GetRequiredService<IMediator>();

                var targetTotal = Math.Max(0, _opt.TargetCount);
                var existingCount = await mediatorStat.Send(new GetMoviesCountQueryRequest(), ct);
                var remaining = Math.Max(0, targetTotal - existingCount);
                if (remaining == 0)
                {
                    _log.LogInformation("MovieSeeder: already have {cnt}/{target}. Nothing to do.", existingCount, targetTotal);
                    return;
                }

                // start page təxmini
                var existingWithTmdb = await mediatorStat.Send(new GetMoviesCountQueryRequest { predicate = m => m.TmdbId != null }, ct);
                const int estUniquePerCombinedPage = 40;
                const int safetyBackPages = 0;
                //var pageGuess = (existingWithTmdb / estUniquePerCombinedPage) - safetyBackPages;
                var pageGuess = 1;
                var maxPages = 5000; // TMDB popular/top_rated üçün real limit
                var page = pageGuess < 1 ? 1 : (pageGuess > maxPages ? ((pageGuess - 1) % maxPages) + 1 : pageGuess);

                _log.LogInformation("MovieSeeder: need to add {rem} (current {curr}, target {target}). Start page ≈ {page}.",
                    remaining, existingCount, targetTotal, page);

                var added = 0;
                var consecutiveNoAdd = 0;
                var seenThisRun = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                // ---- işlək scope: hər 200 insertdən bir yeniləyəcəyik
                AsyncServiceScope workScope = scopeFactory.CreateAsyncScope();
                var mediator = workScope.ServiceProvider.GetRequiredService<IMediator>();
                var inScopeCount = 0;

                try
                {
                    while (added < remaining && !ct.IsCancellationRequested)
                    {
                        var popTask = tmdb.GetPopularAsync(page);
                        //var topTask = tmdb.GetTopRatedAsync(page);
                        await Task.WhenAll(popTask);

                        var pop = popTask.Result ?? new();
                        //var top = topTask.Result ?? new();
                        var batch = pop.Results.GroupBy(r => r.Id).Select(g => g.First()).ToList();

                        if (batch.Count == 0)
                        {
                            consecutiveNoAdd++;
                            page = page >= maxPages ? 1 : page + 1;
                            continue;
                        }

                        var addedThisPage = 0;

                        foreach (var s in batch)
                        {
                            if (added >= remaining) break;

                            var year = ParseYear(s.Release_Date);
                            var uniqKey = $"{s.Title?.Trim()}::{year}";
                            if (!seenThisRun.Add(uniqKey)) continue;

                            var dup = await mediator.Send(new GetMoviesCountQueryRequest
                            {
                                predicate = m => (m.Title == s.Title && m.ReleaseYear == year) || (m.TmdbId == s.Id)
                            }, ct);
                            if (dup > 0) continue;

                            var d = await tmdb.GetDetailsAsync(s.Id);
                            if (d is null || string.IsNullOrWhiteSpace(d.Title)) continue;

                            var cats = GenreMapper.MapToEnumCategories(d.Genres.Select(g => g.Id));
                            if (cats.Count == 0) continue;

                            var cmd = new CreateMovieCommandRequest
                            {
                                SourceVideoUrl = "e6284a84-3054-40e3-994d-eec2909e3f6b",
                                TrailerVideoUrl = await TryYouTubeTrailer(tmdb, d.Id),
                                CoverImageUrl = tmdb.PosterUrl(d.Poster_Path),
                                BannerImageUrl = tmdb.BackdropUrl(d.Backdrop_Path),
                                Title = d.Title,
                                Description = string.IsNullOrWhiteSpace(d.Overview) ? "No description." : d.Overview,
                                ReleaseYear = year,
                                Duration = d.Runtime ?? Random.Shared.Next(85, 140),
                                AgeLimitation = AgeRatingHelper.FromCertOrHeuristic(await tmdb.GetReleaseDatesAsync(d.Id), cats),
                                Categories = cats,
                                IsVisible = true,
                                InitialRating = d.Vote_Average > 0 ? (float)Math.Round(d.Vote_Average, 1) : null,
                                TmdbId = d.Id
                            };

                            try
                            {
                                var response = await mediator.Send(cmd, ct);
                                added++;
                                addedThisPage++;
                                inScopeCount++;

                                if (added % 25 == 0)
                                    _log.LogInformation("MovieSeeder: added {added}/{rem} this run (target total {target}).",
                                        added, remaining, targetTotal);

                                if (added % 40 == 0) await Task.Delay(300, ct);
                            }
                            catch (Exception ex)
                            {
                                _log.LogWarning(ex, "MovieSeeder: CreateMovie failed for {Title} ({Year})", d.Title, year);
                                // burada continue; sayğac artırma!
                            }

                            if (added % 25 == 0)
                                _log.LogInformation("MovieSeeder: added {added}/{rem} this run (target total {target}).",
                                    added, remaining, targetTotal);

                            // kiçik throttle
                            if (added % 40 == 0) await Task.Delay(300, ct);

                            // ---- hər 200 insertdən bir scope-u təzələ (ASYNC!)
                            if (inScopeCount >= 200)
                            {
                                await workScope.DisposeAsync();
                                workScope = scopeFactory.CreateAsyncScope();
                                mediator = workScope.ServiceProvider.GetRequiredService<IMediator>();
                                inScopeCount = 0;
                            }

                            Console.WriteLine("PAGEEEEEEEEEEEEEEEE : " + page);
                            Console.WriteLine("PAGEEEEEEEEEEEEEEEE : " + page);
                            Console.WriteLine("PAGEEEEEEEEEEEEEEEE : " + page);
                            Console.WriteLine("PAGEEEEEEEEEEEEEEEE : " + page);
                            Console.WriteLine("PAGEEEEEEEEEEEEEEEE : " + page);
                            Console.WriteLine("PAGEEEEEEEEEEEEEEEE : " + page);
                            Console.WriteLine("PAGEEEEEEEEEEEEEEEE : " + page);
                            Console.WriteLine("PAGEEEEEEEEEEEEEEEE : " + page);
                            Console.WriteLine("PAGEEEEEEEEEEEEEEEE : " + page);
                            Console.WriteLine("PAGEEEEEEEEEEEEEEEE : " + page);
                            Console.WriteLine("PAGEEEEEEEEEEEEEEEE : " + page);
                            Console.WriteLine("PAGEEEEEEEEEEEEEEEE : " + page);
                            Console.WriteLine("PAGEEEEEEEEEEEEEEEE : " + page);
                            Console.WriteLine("PAGEEEEEEEEEEEEEEEE : " + page);
                            Console.WriteLine("PAGEEEEEEEEEEEEEEEE : " + page);
                            Console.WriteLine("PAGEEEEEEEEEEEEEEEE : " + page);
                            Console.WriteLine("PAGEEEEEEEEEEEEEEEE : " + page);
                            Console.WriteLine("PAGEEEEEEEEEEEEEEEE : " + page);
                            Console.WriteLine("PAGEEEEEEEEEEEEEEEE : " + page);
                        }

                        consecutiveNoAdd = (addedThisPage == 0) ? consecutiveNoAdd + 1 : 0;

                        // 2 ardıcıl boş səhifədən sonra kiçik jump
                        page = (addedThisPage == 0 && consecutiveNoAdd >= 5)
                            ? ((page + 1) > maxPages ? ((page + 1) - maxPages) : (page + 1))
                            : (page >= maxPages ? 1 : page + 1);
                    }
                }
                finally
                {
                    // son scope-u düzgün bağla
                    await workScope.DisposeAsync();
                }

                _log.LogInformation("MovieSeeder finished. Added this run: {added}. Total target: {target}.",
                    added, targetTotal);
            }
        }

        public Task StopAsync(CancellationToken ct) => Task.CompletedTask;

        private static int ParseYear(string? iso)
            => DateTime.TryParse(iso, out var dt) ? dt.Year : Random.Shared.Next(1980, DateTime.UtcNow.Year);

        private static async Task<string?> TryYouTubeTrailer(TmdbClient tmdb, int movieId)
        {
            var vids = await tmdb.GetVideosAsync(movieId);
            var v = vids?.Results?
                .Where(v => v.Site.Equals("YouTube", StringComparison.OrdinalIgnoreCase))
                .OrderBy(v => v.Type != "Trailer")        // Trailer üstün
                .ThenBy(v => v.Type != "Teaser")          // sonra Teaser
                .ThenByDescending(v => v.Official)
                .FirstOrDefault();

            return string.IsNullOrWhiteSpace(v?.Key) ? null : $"https://www.youtube.com/watch?v={v.Key}";
        }
    }
}
