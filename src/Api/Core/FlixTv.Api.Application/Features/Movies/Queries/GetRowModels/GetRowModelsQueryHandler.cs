using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models;
using FlixTv.Common.Models.ResponseModels.Movies;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Movies.Queries.GetRowModels
{
    public class GetRowModelsQueryHandler : IRequestHandler<GetRowModelsQueryRequest, IList<GetRowModelsQueryResponse>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly int userId;

        public GetRowModelsQueryHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            this.unitOfWork = unitOfWork;
            this.userId = Convert.ToInt32(httpContextAccessor.HttpContext?.User?.FindFirst("id")?.Value ?? "0");
        }

        public async Task<IList<GetRowModelsQueryResponse>> Handle(GetRowModelsQueryRequest request, CancellationToken ct)
        {
            var seed = StableSeed(userId);
            var must = new List<GetRowModelsQueryResponse>();
            var pool = new List<(GetRowModelsQueryResponse model, double weight, string diversityKey)>();

            // AUTH YOXDUR → DEFAULT paket
            if (userId <= 0)
            {
                must.Add(MakeSpecial("popular", "Popular on FlixTV", excludeWatched: true, seed));
                must.Add(MakeSpecial("latest", "Latest releases", excludeWatched: true, seed));
                must.Add(MakeSpecial("classics", "Timeless classics", excludeWatched: true, seed));

                var sk = SeasonalKey();
                if (!string.IsNullOrWhiteSpace(sk))
                    pool.Add((MakeSpecial(sk!, PrettyKey(sk!), true, seed), 0.7, $"key:{sk}"));

                pool.Add((MakeSpecial("gems", "Hidden gems", true, seed), 0.6, "key:gems"));

                return Finalize(must, pool, request.Count, seed);
            }

            // AUTH VAR → istifadəçi siqnalları
            var sig = await BuildSignalsAsync(userId, ct);

            // MÜTLƏQ: Continue (əgər var), Popular, Latest
            if (sig.HasInProgress)
            {
                must.Add(new GetRowModelsQueryResponse
                {
                    Type = RowType.ContinueWatching,
                    Title = "Continue watching",
                    RowKey = "continue",
                    ExcludeWatched = false,
                    Seed = seed
                });
            }

            must.Add(MakeSpecial("popular", "Popular on FlixTV", true, seed));
            must.Add(MakeSpecial("latest", "Latest releases", true, seed));

            // TopGenres (tək)
            foreach (var g in sig.FavGenresTop3)
            {
                pool.Add((
                    new GetRowModelsQueryResponse
                    {
                        Type = RowType.TopGenres,
                        Title = TitleForGenre(g),
                        RowKey = $"genre:{g}",
                        ExcludeWatched = true,
                        Genres = new() { g },
                        Seed = seed
                    },
                    1.0,
                    $"genre:{g}"
                ));
            }

            // ComboGenres (iki janr)
            if (sig.FavGenresTop3.Count >= 2)
            {
                for (int i = 0; i < sig.FavGenresTop3.Count; i++)
                    for (int j = i + 1; j < sig.FavGenresTop3.Count; j++)
                    {
                        var a = sig.FavGenresTop3[i];
                        var b = sig.FavGenresTop3[j];
                        pool.Add((
                            new GetRowModelsQueryResponse
                            {
                                Type = RowType.ComboGenres,
                                Title = $"When {GenreName(a)} meets {GenreName(b)}",
                                RowKey = $"combo:{a}-{b}",
                                ExcludeWatched = true,
                                Genres = new() { a, b },
                                Seed = seed
                            },
                            1.1,
                            $"combo:{a}-{b}"
                        ));
                    }
            }

            // SimilarToMovie (son yüksək reytinq verdiyi filmə görə)
            if (sig.LastHighRatedMovieId is int mid)
            {
                var mtitle = await TryGetMovieTitle(mid, ct) ?? "that movie";
                pool.Add((
                    new GetRowModelsQueryResponse
                    {
                        Type = RowType.SimilarToMovie,
                        Title = $"Because you watched {mtitle}",
                        RowKey = $"sim:{mid}",
                        ExcludeWatched = true,
                        SeedMovieId = mid,
                        Seed = seed
                    },
                    1.2,
                    $"sim:{mid}"
                ));
            }

            // Decade (dominant + explore)
            if (sig.DominantDecadeStart is int d)
            {
                pool.Add((
                    new GetRowModelsQueryResponse
                    {
                        Type = RowType.Decade,
                        Title = $"{ShortLabel(d)} binge",
                        RowKey = $"dec:{d}",
                        ExcludeWatched = true,
                        YearFrom = d,
                        YearTo = d + 9,
                        Seed = seed
                    },
                    0.9,
                    $"dec:{d}"
                ));

                var explore = d >= 1990 ? 1980 : 2000;
                pool.Add((
                    new GetRowModelsQueryResponse
                    {
                        Type = RowType.Decade,
                        Title = $"Time travel to the {ShortLabel(explore)}",
                        RowKey = $"dec:{explore}",
                        ExcludeWatched = true,
                        YearFrom = explore,
                        YearTo = explore + 9,
                        Seed = seed
                    },
                    0.7,
                    $"dec:{explore}"
                ));
            }

            // Mövsümi / Classics / Gems → SpecialKey
            var sKey = SeasonalKey();
            if (!string.IsNullOrWhiteSpace(sKey))
                pool.Add((MakeSpecial(sKey!, PrettyKey(sKey!), true, seed), 0.7, $"key:{sKey}"));

            pool.Add((MakeSpecial("classics", "Timeless classics", true, seed), 0.6, "key:classics"));
            pool.Add((MakeSpecial("gems", "Hidden gems for you", true, seed), 0.6, "key:gems"));

            return Finalize(must, pool, request.Count, seed);
        }

        /* ======================= USER SIGNALS ======================= */

        private async Task<UserSignals> BuildSignalsAsync(int uid, CancellationToken ct)
        {
            var sig = new UserSignals();

            // ViewData → Movie (Categories, ReleaseYear) daxil olsun
            var viewRepo = unitOfWork.GetReadRepository<ViewData>();
            var views = await viewRepo.GetAllAsync(
                predicate: v => v.UserId == uid,
                include: q => q.Include(v => v.Movie!), // Categories Movie içindədir
                enableTracking: false
            );

            // Top 3 janr
            var favGenreIds = views
                .Where(v => v.Movie != null && v.Movie.Categories != null)
                .SelectMany(v => v.Movie!.Categories!)
                .Select(cat => (int)cat)
                .GroupBy(id => id)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .Take(3)
                .ToList();

            sig.FavGenresTop3 = favGenreIds;

            // ContinueWatching siqnalı (HAL-HAZIRDA ViewData-da progress sahəsi yoxdur)
            // TODO: ViewData-ya aşağıdakı sahələrdən birini əlavə et, sonra bu şərhi aç:
            //   public int WatchedMinutes { get; set; }
            //   public int LastPositionSeconds { get; set; }
            //   public DateTime? LastWatchedAt { get; set; }
            // və buranı belə yaz:
            // sig.HasInProgress = views.Any(v => v.Movie != null && v.Movie.Duration > 0
            //                                   && v.WatchedMinutes > 0
            //                                   && v.WatchedMinutes < v.Movie.Duration);
            sig.HasInProgress = false;

            // Dominant decade (baxış tarixçəsinə görə orta il)
            var years = views
                .Where(v => v.Movie != null)
                .Select(v => v.Movie!.ReleaseYear)
                .Where(y => y > 0)
                .ToList();

            if (years.Count > 0)
            {
                var avg = (int)Math.Round(years.Average());
                sig.DominantDecadeStart = (avg / 10) * 10;
            }

            // Son yüksək reytinq verdiyi film (Review.AuthorId!)
            var reviewRepo = unitOfWork.GetReadRepository<Review>();
            var lastHigh = await reviewRepo.GetAllByPagingAsync(
                predicate: r => r.AuthorId == uid && r.RatingPoint >= 9,
                orderBy: q => q.OrderByDescending(r => r.CreatedDate),
                currentPage: 1, pageSize: 1,
                enableTracking: false
            );
            sig.LastHighRatedMovieId = lastHigh.FirstOrDefault()?.MovieId;

            return sig;
        }

        private sealed class UserSignals
        {
            public List<int> FavGenresTop3 { get; set; } = new();
            public int? LastHighRatedMovieId { get; set; }
            public bool HasInProgress { get; set; }
            public int? DominantDecadeStart { get; set; }
        }

        /* ======================= BUILDERS ======================= */

        private static GetRowModelsQueryResponse MakeSpecial(string key, string title, bool excludeWatched, int seed) => new()
        {
            Type = RowType.SpecialKey,
            Title = title,
            RowKey = key.StartsWith("key:", StringComparison.OrdinalIgnoreCase) ? key : $"key:{key}",
            Key = key,
            ExcludeWatched = excludeWatched,
            Seed = seed
        };

        /* ======================= PAGE ASSEMBLY ======================= */

        private static IList<GetRowModelsQueryResponse> Finalize(
            List<GetRowModelsQueryResponse> must,
            List<(GetRowModelsQueryResponse model, double weight, string diversity)> pool,
            int count, int seed)
        {
            var rnd = new Random(seed);
            var res = new List<GetRowModelsQueryResponse>();

            // 1) Mütləqlər
            foreach (var m in must)
            {
                if (res.Count >= count) break;
                res.Add(m);
            }

            // 2) Hovuzdan çəkili + müxtəliflik
            var used = new HashSet<string>(must.Select(m => m.RowKey));
            var items = pool.OrderBy(_ => rnd.NextDouble()).ToList();

            foreach (var it in items)
            {
                if (res.Count >= count) break;
                if (used.Contains(it.diversity)) continue;

                var p = 1.0 / (1.0 + Math.Exp(-2 * (it.weight - 0.5))); // sigmoid
                if (rnd.NextDouble() <= p)
                {
                    res.Add(it.model);
                    used.Add(it.diversity);
                }
            }

            // 3) Hələ çatmayıbsa, doldur
            foreach (var it in items)
            {
                if (res.Count >= count) break;
                if (!res.Contains(it.model)) res.Add(it.model);
            }

            // 4) Mütləqlər öndə, qalanlar “stabil” shuffle
            var head = must.Where(res.Contains).ToList();
            var tail = res.Except(head).OrderBy(_ => rnd.Next()).ToList();
            return head.Concat(tail).Take(count).ToList();
        }

        /* ======================= UTILS ======================= */

        private static int StableSeed(int uid) => HashCode.Combine(DateTime.UtcNow.Date, uid, 733);
        private static string ShortLabel(int decade) => $"’{decade % 100}s";

        private static string GenreName(int id)
            => Enum.IsDefined(typeof(MovieCategory), id) ? ((MovieCategory)id).ToString() : "Genre";

        private static string TitleForGenre(int g) => GenreName(g) switch
        {
            nameof(MovieCategory.Action) => "Your Action fix",
            nameof(MovieCategory.Animation) => "Animated picks for you",
            nameof(MovieCategory.Comedy) => "Laugh out loud comedies",
            nameof(MovieCategory.Crime) => "Gritty crime stories",
            nameof(MovieCategory.Drama) => "Must-watch dramas",
            nameof(MovieCategory.Fantasy) => "Epic fantasy worlds",
            nameof(MovieCategory.Historical) => "Historical epics",
            nameof(MovieCategory.Horror) => "Because you like horror",
            nameof(MovieCategory.Romance) => "Romance tonight",
            nameof(MovieCategory.ScienceFiction) => "Sci-Fi adventures",
            nameof(MovieCategory.Thriller) => "Edge-of-seat thrillers",
            nameof(MovieCategory.Western) => "Classic western vibes",
            _ => $"Because you like {GenreName(g)}"
        };

        private static string? SeasonalKey()
        {
            var m = DateTime.UtcNow.Month;
            if (m == 10) return "SpookySeason";
            if (m == 12) return "HolidayPicks";
            if (m == 2) return "AwardsSeason";
            return null;
        }

        private static string PrettyKey(string k) => k switch
        {
            "SpookySeason" => "Spooky season picks",
            "HolidayPicks" => "Holiday picks",
            "AwardsSeason" => "Awards season favorites",
            _ => k
        };

        private async Task<string?> TryGetMovieTitle(int movieId, CancellationToken ct)
        {
            var movieRepo = unitOfWork.GetReadRepository<Movie>();
            var movie = await movieRepo.GetAsync(m => m.Id == movieId, enableTracking: false);
            return movie?.Title;
        }
    }
}
