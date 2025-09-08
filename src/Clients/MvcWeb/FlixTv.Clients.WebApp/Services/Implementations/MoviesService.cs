using FlixTv.Clients.WebApp.Services.Abstractions;
using FlixTv.Clients.WebApp.Services.Http;
using FlixTv.Common.Models;
using FlixTv.Common.Models.ResponseModels.Movies;
using System.Text;

namespace FlixTv.Clients.WebApp.Services.Implementations
{
    public class MoviesService : BaseApiClient, IMoviesService
    {
        private const string RowsEndpoint = "Movies/GetRowModels";
        private const string GetAllMoviesEndpoint = "Movies/GetAllMovies";
        private const string RelatedEndpoint = "Movies/GetRelatedMovies";
        private const string UnfinishedEndpoint = "Movies/GetUnfinishedMovies";
        private const string CompatibilityEndpoint = "Movies/GetMoviesByUserCompatibility";
        private const string GetMovieEndpoint = "Movies/GetMovie";


        public MoviesService(IHttpClientFactory factory)
            : base(factory.CreateClient("flix-api")) { }

        public Task<ApiResult<GetMovieQueryResponse>> GetMovieAsync(
            int id, 
            CancellationToken ct = default)
        {
            if (id <= 0)
                return Task.FromResult(
                    ApiResult<GetMovieQueryResponse>.Fail(
                        new[] { "Invalid movie id." }, System.Net.HttpStatusCode.BadRequest));

            var url = $"{GetMovieEndpoint}/{id}"; // api/Movies/GetMovie/{id}
            return GetAsync<GetMovieQueryResponse>(url, ct);
        }

        public Task<ApiResult<IList<GetAllMoviesQueryResponse>>> GetRelatedMoviesAsync(
            int movieId,
            CancellationToken ct = default)
        {
            if (movieId <= 0)
                return Task.FromResult(
                    ApiResult<IList<GetAllMoviesQueryResponse>>.Fail(
                        new[] { "Invalid movie id." },
                        System.Net.HttpStatusCode.BadRequest));

            var url = $"{RelatedEndpoint}?movieId={movieId}";
            return GetAsync<IList<GetAllMoviesQueryResponse>>(url, ct);
        }

        public Task<ApiResult<IList<GetRowModelsQueryResponse>>> GetRowModelsAsync(
            int count = 12,
            CancellationToken ct = default)
        {
            var url = $"{RowsEndpoint}?count={count}";
            return GetAsync<IList<GetRowModelsQueryResponse>>(url, ct);
        }

        public Task<ApiResult<IList<GetAllMoviesQueryResponse>>> GetMoviesByUserCompatibilityAsync(
            int count = 10, 
            CancellationToken ct = default)
        {
            var url = $"{CompatibilityEndpoint}?count={count}";
            return GetAsync<IList<GetAllMoviesQueryResponse>>(url, ct);
        }

        public async Task<ApiResult<IList<GetAllMoviesQueryResponse>>> GetMoviesForRowAsync(
            GetRowModelsQueryResponse row,
            int count = 12,
            CancellationToken ct = default)
        {
            switch (row.Type)
            {
                case RowType.SimilarToMovie:
                    {
                        if (row.SeedMovieId is not int mid)
                            return ApiResult<IList<GetAllMoviesQueryResponse>>.Fail(new[] { "SeedMovieId does not exist." }, System.Net.HttpStatusCode.BadRequest);

                        var url = $"{RelatedEndpoint}?movieId={mid}";
                        return await GetAsync<IList<GetAllMoviesQueryResponse>>(url, ct);
                    }

                case RowType.ContinueWatching:
                    {
                        return await GetAsync<IList<GetAllMoviesQueryResponse>>(UnfinishedEndpoint, ct);
                    }

                case RowType.Decade:
                    {
                        var qs = new Dictionary<string, string?>
                        {
                            ["minReleaseYear"] = row.YearFrom?.ToString(),
                            ["maxReleaseYear"] = row.YearTo?.ToString(),
                            ["orderBy"] = "rating",
                            ["currentPage"] = "1",
                            ["pageSize"] = count.ToString()
                        };
                        var url = BuildGetAllMoviesUrl(qs, categories: null);
                        return await GetAsync<IList<GetAllMoviesQueryResponse>>(url, ct);
                    }

                case RowType.TopGenres:
                case RowType.ComboGenres:
                    {
                        var catNames = ToCategoryNames(row.Genres);
                        var qs = new Dictionary<string, string?>
                        {
                            ["orderBy"] = "rating",
                            ["currentPage"] = "1",
                            ["pageSize"] = count.ToString()
                        };
                        var url = BuildGetAllMoviesUrl(qs, categories: catNames);
                        return await GetAsync<IList<GetAllMoviesQueryResponse>>(url, ct);
                    }

                case RowType.SpecialKey:
                default:
                    {
                        var (qs, cats) = BuildSpecialKeyQuery(row.Key, count);
                        var url = BuildGetAllMoviesUrl(qs, cats);
                        return await GetAsync<IList<GetAllMoviesQueryResponse>>(url, ct);
                    }
            }
        }

        public Task<ApiResult<IList<GetAllMoviesQueryResponse>>> GetLatestMoviesAsync(
            int pool = 50,
            CancellationToken ct = default)
        {
            var qs = new Dictionary<string, string?>
            {
                ["orderBy"] = "releaseYear",
                ["currentPage"] = "1",
                ["pageSize"] = Math.Max(1, pool).ToString()
            };

            qs["minReleaseYear"] = (DateTime.UtcNow.Year - 2).ToString();

            var url = BuildGetAllMoviesUrl(qs, categories: null);
            return GetAsync<IList<GetAllMoviesQueryResponse>>(url, ct);
        }

        private static (
            Dictionary<string, string?> qs, 
            List<string>? categories)
            BuildSpecialKeyQuery(string? key, int count)
        {
            var nowYear = DateTime.UtcNow.Year;
            var qs = new Dictionary<string, string?>
            {
                ["currentPage"] = "1",
                ["pageSize"] = count.ToString()
            };
            List<string>? cats = null;

            var k = (key ?? "").Trim().ToLowerInvariant();

            switch (k)
            {
                case "popular":
                case "trending":
                    qs["orderBy"] = "popular";
                    qs["minReleaseYear"] = (nowYear - 10).ToString();
                    break;

                case "latest":
                case "new":
                    qs["minReleaseYear"] = (nowYear - 1).ToString();
                    qs["orderBy"] = "releaseYear";
                    break;

                case "classics":
                    qs["maxReleaseYear"] = (nowYear - 15).ToString();
                    qs["minRating"] = "8";
                    qs["orderBy"] = "rating";
                    break;

                case "gems":
                    qs["minRating"] = "7";
                    qs["maxViewCount"] = "50";
                    qs["orderBy"] = "rating";
                    break;

                case "spookyseason":
                    cats = new List<string> { nameof(MovieCategory.Horror) };
                    qs["orderBy"] = "rating";
                    break;

                case "holidaypicks":
                    cats = new List<string> { nameof(MovieCategory.Fantasy), nameof(MovieCategory.Romance) };
                    qs["ageLimitation"] = "16";
                    qs["orderBy"] = "rating";
                    break;

                case "awardsseason":
                    cats = new List<string> { nameof(MovieCategory.Drama) };
                    qs["minRating"] = "8";
                    qs["orderBy"] = "rating";
                    break;

                default:
                    qs["orderBy"] = "rating";
                    break;
            }

            return (qs, cats);
        }

        private static string BuildGetAllMoviesUrl(
            IDictionary<string, string?> qs,
            IEnumerable<string>? categories)
        {
            var sb = new StringBuilder();
            sb.Append(GetAllMoviesEndpoint);
            sb.Append('?');

            var first = true;

            foreach (var kv in qs)
            {
                if (string.IsNullOrWhiteSpace(kv.Value)) continue;
                if (!first) sb.Append('&'); first = false;
                sb.Append(Uri.EscapeDataString(kv.Key));
                sb.Append('=');
                sb.Append(Uri.EscapeDataString(kv.Value!));
            }

            if (categories != null)
            {
                foreach (var cat in categories)
                {
                    if (string.IsNullOrWhiteSpace(cat)) continue;
                    if (!first) sb.Append('&'); first = false;
                    sb.Append("Categories=");
                    sb.Append(Uri.EscapeDataString(cat));
                }
            }

            return sb.ToString();
        }

        private static List<string> ToCategoryNames(
            List<int>? genreIds)
        {
            var list = new List<string>();
            if (genreIds is null) return list;

            foreach (var id in genreIds)
            {
                if (Enum.IsDefined(typeof(MovieCategory), id))
                {
                    var name = Enum.GetName(typeof(MovieCategory), id);
                    if (!string.IsNullOrWhiteSpace(name)) list.Add(name!);
                }
            }
            return list;
        }
    }
}
