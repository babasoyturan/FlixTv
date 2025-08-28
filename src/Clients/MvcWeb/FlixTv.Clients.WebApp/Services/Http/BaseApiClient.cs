using System.Net;
using System.Text;
using System.Text.Json;

namespace FlixTv.Clients.WebApp.Services.Http
{
    public class BaseApiClient
    {
        protected readonly HttpClient Http;
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web)
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public BaseApiClient(HttpClient http) => Http = http;

        // GET => ApiResult<T>
        protected async Task<ApiResult<T>> GetAsync<T>(string url, CancellationToken ct = default)
        {
            try
            {
                var resp = await Http.GetAsync(url, ct);
                if (resp.IsSuccessStatusCode)
                {
                    var dto = await SafeReadJson<T>(resp, ct);
                    return ApiResult<T>.Success(dto, resp.StatusCode);
                }
                var errors = await ReadErrorsAsync(resp, ct);
                return ApiResult<T>.Fail(errors, resp.StatusCode);
            }
            catch (Exception ex)
            {
                return ApiResult<T>.Fail(new[] { "Service temporarily unavailable.", ex.Message }, HttpStatusCode.ServiceUnavailable);
            }
        }

        // POST JSON body => ApiResult<TOut>
        protected async Task<ApiResult<TOut>> PostJsonAsync<TIn, TOut>(string url, TIn body, CancellationToken ct = default)
        {
            try
            {
                var resp = await Http.PostAsJsonAsync(url, body, _json, ct);
                if (resp.IsSuccessStatusCode)
                {
                    var dto = await SafeReadJson<TOut>(resp, ct);
                    return ApiResult<TOut>.Success(dto, resp.StatusCode);
                }
                var errors = await ReadErrorsAsync(resp, ct);
                return ApiResult<TOut>.Fail(errors, resp.StatusCode);
            }
            catch (Exception ex) // BrokenCircuitException/HttpRequestException/TaskCanceledException
            {
                return ApiResult<TOut>.Fail(new[] { "Service temporarily unavailable.", ex.Message }, HttpStatusCode.ServiceUnavailable);
            }
        }

        // POST (multipart və ya boş content) => ApiResult<bool>
        protected async Task<ApiResult<bool>> PostAsync(string url, HttpContent? content = null, CancellationToken ct = default)
        {
            try
            {
                var resp = await Http.PostAsync(url, content ?? new StringContent(string.Empty, Encoding.UTF8, "application/json"), ct);
                if (resp.IsSuccessStatusCode) return ApiResult<bool>.Success(true, resp.StatusCode);
                var errors = await ReadErrorsAsync(resp, ct);
                return ApiResult<bool>.Fail(errors, resp.StatusCode);
            }
            catch (Exception ex)
            {
                return ApiResult<bool>.Fail(new[] { "Service temporarily unavailable.", ex.Message }, HttpStatusCode.ServiceUnavailable);
            }
        }

        private static async Task<T?> SafeReadJson<T>(HttpResponseMessage resp, CancellationToken ct)
        {
            try { return await resp.Content.ReadFromJsonAsync<T>(_json, ct); }
            catch { return default; }
        }

        // API-nin error formatları:
        // 1) ["err1","err2"]
        // 2) { "errors": ["..."] } / { "Errors": [...] }
        // 3) ProblemDetails { title, detail }
        // 4) Fallback: xam mətn
        private static async Task<List<string>> ReadErrorsAsync(HttpResponseMessage resp, CancellationToken ct)
        {
            var buf = await resp.Content.ReadAsStringAsync(ct);
            if (string.IsNullOrWhiteSpace(buf))
                return [$"{(int)resp.StatusCode} {resp.ReasonPhrase}"];

            // 1) plain string list
            if (TryDeserialize<List<string>>(buf, out var list) && list is { Count: > 0 })
                return list;

            // 2) envelope { errors: [...] } / { Errors: [...] }
            if (TryDeserialize<ErrorsEnvelope>(buf, out var env))
            {
                if (env?.Errors is { Count: > 0 }) return env.Errors!;
                if (env?.errors is { Count: > 0 }) return env.errors!;
            }

            // 3) ProblemDetails
            if (TryDeserialize<ProblemDetailsLike>(buf, out var pd))
            {
                var outList = new List<string>();
                if (!string.IsNullOrWhiteSpace(pd?.Title)) outList.Add(pd!.Title!);
                if (!string.IsNullOrWhiteSpace(pd?.Detail)) outList.Add(pd!.Detail!);
                if (outList.Count > 0) return outList;
            }

            // 4) Fallback
            return [buf];

            static bool TryDeserialize<TAny>(string json, out TAny? value)
            {
                try { value = JsonSerializer.Deserialize<TAny>(json, new JsonSerializerOptions(JsonSerializerDefaults.Web)); return true; }
                catch { value = default; return false; }
            }
        }

        private sealed class ErrorsEnvelope
        {
            public List<string>? errors { get; set; }
            public List<string>? Errors { get; set; }
        }

        private sealed class ProblemDetailsLike
        {
            public string? Title { get; set; }
            public string? Detail { get; set; }
            public int? Status { get; set; }
        }
    }
}
