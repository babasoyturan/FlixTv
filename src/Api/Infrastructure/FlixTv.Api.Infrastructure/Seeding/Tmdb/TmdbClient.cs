using FlixTv.Api.Infrastructure.Seeding.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FlixTv.Api.Infrastructure.Seeding.Tmdb
{
    public sealed class TmdbClient
    {
        private readonly HttpClient _http;
        private readonly TmdbOptions _opt;
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public TmdbClient(HttpClient http, IOptions<TmdbOptions> options)
        {
            _http = http;
            _opt = options.Value;
        }

        public Task<TmdbPaged<TmdbMovieSummary>?> GetPopularAsync(int page)
            => GetAsync<TmdbPaged<TmdbMovieSummary>>($"movie/popular?page={page}");

        public Task<TmdbPaged<TmdbMovieSummary>?> GetTopRatedAsync(int page)
            => GetAsync<TmdbPaged<TmdbMovieSummary>>($"movie/top_rated?page={page}");

        public Task<TmdbPaged<TmdbMovieSummary>?> DiscoverAsync(int page, int? year = null, int voteCountGte = 1)
            => GetAsync<TmdbPaged<TmdbMovieSummary>>(
                $"discover/movie?page={page}&sort_by=popularity.desc&vote_count.gte={voteCountGte}" +
                (year.HasValue ? $"&primary_release_year={year.Value}" : "")
            );

        public Task<TmdbMovieDetails?> GetDetailsAsync(int id)
            => GetAsync<TmdbMovieDetails>($"movie/{id}");

        public Task<TmdbVideos?> GetVideosAsync(int id)
            => GetAsync<TmdbVideos>($"movie/{id}/videos");

        public Task<TmdbReleaseDates?> GetReleaseDatesAsync(int id)
            => GetAsync<TmdbReleaseDates>($"movie/{id}/release_dates");

        public Task<TmdbPaged<TmdbMovieSummary>?> GetSimilarAsync(int id, int page = 1)
            => GetAsync<TmdbPaged<TmdbMovieSummary>>($"movie/{id}/similar?page={page}");

        public Task<TmdbPaged<TmdbMovieSummary>?> GetRecommendationsAsync(int id, int page = 1)
            => GetAsync<TmdbPaged<TmdbMovieSummary>>($"movie/{id}/recommendations?page={page}");

        private async Task<T?> GetAsync<T>(string url, int attempts = 4, CancellationToken ct = default)
        {
            for (var i = 0; i < attempts; i++)
            {
                try
                {
                    using var resp = await _http.GetAsync(url, ct);
                    if ((int)resp.StatusCode == 429)
                    {
                        var wait = resp.Headers.RetryAfter?.Delta ?? TimeSpan.FromSeconds(1 << i);
                        await Task.Delay(wait, ct);
                        continue;
                    }
                    resp.EnsureSuccessStatusCode();
                    return await resp.Content.ReadFromJsonAsync<T>(_json, ct);
                }
                catch (HttpRequestException) when (i < attempts - 1)
                { await Task.Delay(250 * (1 << i), ct); }
                catch (IOException) when (i < attempts - 1)
                { await Task.Delay(250 * (1 << i), ct); }
            }
            return default;
        }

        public string PosterUrl(string? path)
            => string.IsNullOrWhiteSpace(path) ? "" : $"{_opt.ImageBase}{_opt.PosterSize}{path}";
        public string BackdropUrl(string? path)
            => string.IsNullOrWhiteSpace(path) ? "" : $"{_opt.ImageBase}{_opt.BackdropSize}{path}";

        // ===== DTO-lar =====
        public sealed class TmdbPaged<T>
        {
            public int Page { get; set; }
            public List<T> Results { get; set; } = new();
            public int Total_Pages { get; set; }
        }

        public sealed class TmdbMovieSummary
        {
            public int Id { get; set; }
            public string Title { get; set; } = "";
            public string Overview { get; set; } = "";
            public string Release_Date { get; set; } = "";
            public string Poster_Path { get; set; } = "";
            public string Backdrop_Path { get; set; } = "";
            public double Vote_Average { get; set; }
            public List<int> Genre_Ids { get; set; } = new();
        }

        public sealed class TmdbMovieDetails
        {
            public int Id { get; set; }
            public string Title { get; set; } = "";
            public string Overview { get; set; } = "";
            public string Release_Date { get; set; } = "";
            public int? Runtime { get; set; }
            public string Poster_Path { get; set; } = "";
            public string Backdrop_Path { get; set; } = "";
            public double Vote_Average { get; set; }
            public List<TmdbGenre> Genres { get; set; } = new();
        }
        public sealed class TmdbGenre { public int Id { get; set; } public string Name { get; set; } = ""; }

        public sealed class TmdbVideos { public List<TmdbVideo> Results { get; set; } = new(); }
        public sealed class TmdbVideo
        {
            public string Key { get; set; } = "";   // YouTube key
            public string Site { get; set; } = "";  // YouTube
            public string Type { get; set; } = "";  // Trailer, Teaser
            public bool Official { get; set; }
        }

        public sealed class TmdbReleaseDates
        {
            public List<TmdbReleaseDatesResult> Results { get; set; } = new();
        }
        public sealed class TmdbReleaseDatesResult
        {
            public string Iso_3166_1 { get; set; } = "";
            public List<TmdbReleaseDate> Release_Dates { get; set; } = new();
        }
        public sealed class TmdbReleaseDate
        {
            public string Certification { get; set; } = ""; // PG-13, R, 12A...
        }
    }
}
