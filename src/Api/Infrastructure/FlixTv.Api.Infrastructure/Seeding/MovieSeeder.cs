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
            if (!_opt.Enabled)
            {
                _log.LogInformation("MovieSeeder disabled.");
                return;
            }

            using var scope = _sp.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var tmdb = scope.ServiceProvider.GetRequiredService<TmdbClient>();

            // 1) Ümumi hədəf – yalnız çatmayan qədər seed et
            var targetTotal = Math.Max(0, _opt.TargetCount);
            var existingCount = await mediator.Send(new GetMoviesCountQueryRequest(), ct);
            var remaining = Math.Max(0, targetTotal - existingCount);

            if (remaining == 0)
            {
                _log.LogInformation("MovieSeeder: already have {cnt}/{target}. Nothing to do.", existingCount, targetTotal);
                return;
            }

            _log.LogInformation("MovieSeeder: need to add {rem} (current {curr}, target {target}).",
                remaining, existingCount, targetTotal);

            var added = 0;
            var page = 1;
            var maxPages = 500;                 // sərt limit – təhlükəsizlik
            var consecutiveNoAdd = 0;
            var seenThisRun = new HashSet<string>(StringComparer.OrdinalIgnoreCase); // title::year

            while (added < remaining && page <= maxPages && consecutiveNoAdd < 8 && !ct.IsCancellationRequested)
            {
                // Eyni səhifədə popular + top_rated; dublikat TMDB id-ləri birləşdir
                var popTask = tmdb.GetPopularAsync(page);
                var topTask = tmdb.GetTopRatedAsync(page);
                await Task.WhenAll(popTask, topTask);

                var pop = popTask.Result ?? new();
                var top = topTask.Result ?? new();

                var batch = pop.Results.Concat(top.Results)
                                       .GroupBy(r => r.Id)
                                       .Select(g => g.First())
                                       .ToList();

                if (batch.Count == 0)
                {
                    consecutiveNoAdd++;
                    page++;
                    continue;
                }

                var addedThisPage = 0;

                foreach (var s in batch)
                {
                    if (added >= remaining) break;

                    var year = ParseYear(s.Release_Date);
                    var uniqKey = $"{s.Title?.Trim()}::{year}";
                    if (!seenThisRun.Add(uniqKey)) continue;

                    // DB-də artıq varmı? (title+year) və ya (TmdbId)
                    var dup = await mediator.Send(new GetMoviesCountQueryRequest
                    {
                        predicate = m => (m.Title == s.Title && m.ReleaseYear == year) || (m.TmdbId == s.Id)
                    }, ct);
                    if (dup > 0) continue;

                    // Detallar
                    var d = await tmdb.GetDetailsAsync(s.Id);
                    if (d is null || string.IsNullOrWhiteSpace(d.Title)) continue;

                    // Janrları xəritələ
                    var cats = GenreMapper.MapToEnumCategories(d.Genres.Select(g => g.Id));
                    if (cats.Count == 0) continue;

                    // Mediyalar
                    var poster = tmdb.PosterUrl(d.Poster_Path);
                    var banner = tmdb.BackdropUrl(d.Backdrop_Path);
                    var trailer = await TryYouTubeTrailer(tmdb, d.Id);
                    var rel = await tmdb.GetReleaseDatesAsync(d.Id);
                    var age = AgeRatingHelper.FromCertOrHeuristic(rel, cats);

                    var cmd = new CreateMovieCommandRequest
                    {
                        SourceVideoUrl = "e6284a84-3054-40e3-994d-eec2909e3f6b",   // placeholder/video id
                        TrailerVideoUrl = trailer,
                        CoverImageUrl = poster,
                        BannerImageUrl = banner,
                        Title = d.Title,
                        Description = string.IsNullOrWhiteSpace(d.Overview) ? "No description." : d.Overview,
                        ReleaseYear = year,
                        Duration = d.Runtime ?? Random.Shared.Next(85, 140),
                        AgeLimitation = age,
                        Categories = cats,
                        IsVisible = true,
                        InitialRating = d.Vote_Average > 0 ? (float)Math.Round(d.Vote_Average, 1) : null,
                        // ✨ TmdbId – handler-də Movie.TmdbId-ə kopyalanmalıdır
                        TmdbId = d.Id
                    };

                    try
                    {
                        await mediator.Send(cmd, ct);
                        added++;
                        addedThisPage++;

                        if (added % 25 == 0)
                            _log.LogInformation("MovieSeeder: added {added}/{rem} this run (target total {target}).",
                                added, remaining, targetTotal);

                        // kiçik throttle – TMDB rate limit
                        if (added % 40 == 0)
                            await Task.Delay(300, ct);
                    }
                    catch (Exception ex)
                    {
                        _log.LogWarning(ex, "MovieSeeder: CreateMovie failed for {Title} ({Year})", d.Title, year);
                    }

                    // Təhlükəsizlik: ümumi say limiti aşılıbsa dayan
                    if (added % 40 == 0)
                    {
                        var totalNow = await mediator.Send(new GetMoviesCountQueryRequest(), ct);
                        if (totalNow >= targetTotal)
                        {
                            _log.LogInformation("MovieSeeder: reached total target {target}. Stopping.", targetTotal);
                            break;
                        }
                    }
                }

                consecutiveNoAdd = (addedThisPage == 0) ? consecutiveNoAdd + 1 : 0;
                page++;
            }

            _log.LogInformation("MovieSeeder finished. Added this run: {added}. Total target: {target}.",
                added, targetTotal);
        }

        public Task StopAsync(CancellationToken ct) => Task.CompletedTask;

        private static int ParseYear(string? iso)
            => DateTime.TryParse(iso, out var dt) ? dt.Year : Random.Shared.Next(1980, DateTime.UtcNow.Year);

        private static async Task<string> TryYouTubeTrailer(TmdbClient tmdb, int movieId)
        {
            var vids = await tmdb.GetVideosAsync(movieId);
            var tr = vids?.Results
                .Where(v => v.Site.Equals("YouTube", StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(v => v.Official)
                .ThenBy(v => v.Type == "Trailer" ? 0 : 1)
                .FirstOrDefault();
            return tr is null ? "" : $"https://www.youtube.com/watch?v={tr.Key}";
        }
    }
}
