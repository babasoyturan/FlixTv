using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Api.Infrastructure.Seeding.Options;
using FlixTv.Api.Infrastructure.Seeding.Tmdb;
using Microsoft.EntityFrameworkCore;
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
    public sealed class SimilarMoviesSeeder : IHostedService
    {
        private readonly ILogger<SimilarMoviesSeeder> _log;
        private readonly IServiceProvider _sp;
        private readonly SimilaritiesSeedingOptions _opt;

        public SimilarMoviesSeeder(
            ILogger<SimilarMoviesSeeder> log,
            IServiceProvider sp,
            IOptions<SimilaritiesSeedingOptions> opt)
        {
            _log = log;
            _sp = sp;
            _opt = opt.Value;
        }

        public async Task StartAsync(CancellationToken ct)
        {
            if (!_opt.Enabled)
            {
                _log.LogInformation("Similarities seeder disabled.");
                return;
            }

            using var scope = _sp.CreateScope();

            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var tmdb = scope.ServiceProvider.GetRequiredService<TmdbClient>();

            var movies = await uow.GetReadRepository<Movie>()
                .GetAllAsync(
                    predicate: m => m.TmdbId != null,
                    include: q => q.Include(m => m.SimilarMovies),
                    enableTracking: true);

            if (movies.Count == 0)
            {
                _log.LogInformation("Similarities: no movies with TmdbId. Exit.");
                return;
            }

            var byTmdb = movies
                .Where(m => m.TmdbId.HasValue)
                .ToDictionary(m => m.TmdbId!.Value, m => m);

            int processed = 0;
            int linksAdded = 0;
            int dirtyBatch = 0;
            var perLimit = Math.Max(1, _opt.PerMovieLimit);

            foreach (var m in movies)
            {
                if (ct.IsCancellationRequested) break;
                processed++;
                if (m.TmdbId is null) continue;

                try
                {
                    var rec = await tmdb.GetRecommendationsAsync(m.TmdbId.Value, 1);
                    var ids = rec?.Results?.Select(r => r.Id).Distinct() ?? Enumerable.Empty<int>();

                    m.SimilarMovies ??= new List<Movie>();

                    foreach (var recTmdbId in ids)
                    {
                        if (m.SimilarMovies.Count >= perLimit) break;
                        if (!byTmdb.TryGetValue(recTmdbId, out var other)) continue;
                        if (other.Id == m.Id) continue;

                        other.SimilarMovies ??= new List<Movie>();

                        // artıq bağlıdırsa keç
                        if (m.SimilarMovies.Any(x => x.Id == other.Id) ||
                            other.SimilarMovies.Any(x => x.Id == m.Id))
                            continue;

                        // hər iki tərəfdə limit
                        if (other.SimilarMovies.Count >= perLimit) continue;

                        m.SimilarMovies.Add(other);
                        other.SimilarMovies.Add(m);
                        linksAdded++;
                        dirtyBatch++;

                        // yüngül throttle
                        if (linksAdded % 120 == 0)
                            await Task.Delay(150, ct);
                    }
                }
                catch (Exception ex)
                {
                    _log.LogWarning(ex, "Similarities: failed for movie {Id}:{Title}", m.Id, m.Title);
                }

                // yalnız real dəyişiklik olanda saxla
                if (dirtyBatch >= 50)
                {
                    await uow.SaveAsync();
                    _log.LogInformation("Similarities: progress {p}/{t}, links added {l}",
                        processed, movies.Count, linksAdded);
                    dirtyBatch = 0;
                }
            }

            if (dirtyBatch > 0)
                await uow.SaveAsync();

            _log.LogInformation("Similarities finished. Processed {p}, links added {l}.",
                processed, linksAdded);
        }

        public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
    }
}
