using FlixTv.Common.Models.ResponseModels.Auth;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace FlixTv.Clients.WebApp.Services.Http
{
    public sealed class RefreshTokenHandler : DelegatingHandler
    {
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();
        private readonly IHttpClientFactory _httpFactory;
        private readonly IHttpContextAccessor _ctx;
        private readonly IPerRequestTokenStore _store;

        private static readonly HttpRequestOptionsKey<string> BodyKey = new("__req_body");
        private static readonly HttpRequestOptionsKey<string> MediaTypeKey = new("__req_media");
        private static readonly TimeSpan ProactiveWindow = TimeSpan.FromSeconds(30);

        public RefreshTokenHandler(IHttpClientFactory httpFactory, IHttpContextAccessor ctx, IPerRequestTokenStore store)
        {
            _httpFactory = httpFactory; _ctx = ctx; _store = store;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            await BufferContentIfTextAsync(request, ct);

            // --- 0) Tokenləri store→cookie qaydasında oxu
            var access = _store.AccessToken ?? _ctx.HttpContext?.Request.Cookies["flix_access_token"];
            var refresh = _store.RefreshToken ?? _ctx.HttpContext?.Request.Cookies["flix_refresh_token"];
            if (!string.IsNullOrWhiteSpace(access) && string.IsNullOrWhiteSpace(_store.AccessToken)) _store.AccessToken = access;
            if (!string.IsNullOrWhiteSpace(refresh) && string.IsNullOrWhiteSpace(_store.RefreshToken)) _store.RefreshToken = refresh;

            // --- 1) Proaktiv refresh (məs: bitməyə 30s qalıbsa)
            if (!string.IsNullOrWhiteSpace(_store.AccessToken) && !string.IsNullOrWhiteSpace(_store.RefreshToken)
                && IsExpiringSoon(_store.AccessToken, ProactiveWindow))
            {
                var tokens = await TryRefreshSingleFlightAsync(_store.RefreshToken!, ct);
                if (tokens is not null)
                {
                    SaveTokensToStoreAndCookies(tokens);
                    ApplyAuthorizationHeader(request, tokens.AccessToken);
                }
            }

            // --- 2) Sorğunu göndər
            var alreadyRetried = request.Headers.Contains("X-Retried-After-Refresh");
            var response = await base.SendAsync(request, ct);

            // --- 3) Yalnız EXPİRED hallarında 401-dən sonra refresh
            if (response.StatusCode != HttpStatusCode.Unauthorized || alreadyRetried)
                return response;

            access = _store.AccessToken;
            refresh = _store.RefreshToken;
            if (string.IsNullOrWhiteSpace(access) || string.IsNullOrWhiteSpace(refresh))
                return response;

            var shouldRefresh = IsExpired(access) || WwwAuthenticateSaysExpired(response.Headers.WwwAuthenticate);
            if (!shouldRefresh) return response;

            var newTokens = await TryRefreshSingleFlightAsync(refresh!, ct);
            if (newTokens is null) return response;

            SaveTokensToStoreAndCookies(newTokens);

            var clone = Clone(request);
            ApplyAuthorizationHeader(clone, newTokens.AccessToken);
            clone.Headers.Add("X-Retried-After-Refresh", "1");
            return await base.SendAsync(clone, ct);
        }

        // ===== helpers =====
        private async Task<RefreshTokenCommandResponse?> TryRefreshSingleFlightAsync(string refreshToken, CancellationToken ct)
        {
            // eyni istifadəçi/sessiya üçün bir refresh
            var key = refreshToken; // istəsən UserId claim-i ilə də açarlaya bilərsən
            var gate = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
            await gate.WaitAsync(ct);
            try
            {
                // Bu anda store-da kimsə artıq yeniləyibsə, təkrar etməyə ehtiyac yoxdur
                var access = _store.AccessToken;
                if (!string.IsNullOrWhiteSpace(access) && !IsExpiringSoon(access, ProactiveWindow) && !IsExpired(access))
                    return null;

                var authClient = _httpFactory.CreateClient("flix-auth");
                var resp = await authClient.PostAsJsonAsync("Auth/RefreshToken", new
                {
                    AccessToken = _store.AccessToken,
                    RefreshToken = _store.RefreshToken
                }, ct);

                if (!resp.IsSuccessStatusCode) return null;
                return await resp.Content.ReadFromJsonAsync<RefreshTokenCommandResponse>(cancellationToken: ct);
            }
            finally
            {
                gate.Release();
                _locks.TryRemove(key, out _); // köhnə açarı təmizlə
            }
        }

        private static void ApplyAuthorizationHeader(HttpRequestMessage req, string accessToken)
        {
            req.Headers.Remove("Authorization");
            req.Headers.TryAddWithoutValidation("Authorization", $"Bearer {accessToken}");
        }

        private void SaveTokensToStoreAndCookies(RefreshTokenCommandResponse data)
        {
            // 1) Request-ölçülü store
            _store.AccessToken = data.AccessToken;
            _store.RefreshToken = data.RefreshToken;

            // 2) Növbəti browser request-ləri üçün cookie
            var ctx = _ctx.HttpContext!;
            var opts = new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Lax, Expires = data.Expiration };
            ctx.Response.Cookies.Append("flix_access_token", data.AccessToken, opts);
            ctx.Response.Cookies.Append("flix_refresh_token", data.RefreshToken, opts);
        }

        private static bool IsExpired(string jwt) => TryGetExp(jwt, out var exp) && DateTimeOffset.UtcNow >= exp;
        private static bool IsExpiringSoon(string jwt, TimeSpan window) => TryGetExp(jwt, out var exp) && (exp - DateTimeOffset.UtcNow) <= window;

        private static bool TryGetExp(string jwt, out DateTimeOffset exp)
        {
            exp = default;
            try { var t = new JwtSecurityTokenHandler().ReadJwtToken(jwt); exp = new DateTimeOffset(t.ValidTo, TimeSpan.Zero); return true; }
            catch { return false; }
        }

        private static bool WwwAuthenticateSaysExpired(HttpHeaderValueCollection<AuthenticationHeaderValue> www)
        {
            foreach (var h in www)
            {
                if (!"Bearer".Equals(h.Scheme, StringComparison.OrdinalIgnoreCase)) continue;
                var p = h.Parameter ?? "";
                if (p.Contains("invalid_token", StringComparison.OrdinalIgnoreCase) &&
                    p.Contains("expired", StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        private static async Task BufferContentIfTextAsync(HttpRequestMessage request, CancellationToken ct)
        {
            var content = request.Content;
            if (content is null) return;

            var mt = content.Headers.ContentType?.MediaType?.ToLowerInvariant();
            var isTextual = mt is not null && (mt.StartsWith("application/json") || mt.StartsWith("text/"));
            if (!isTextual) return;

            await content.LoadIntoBufferAsync();
            var body = await content.ReadAsStringAsync(ct);
            request.Options.Set(BodyKey, body);
            request.Options.Set(MediaTypeKey, mt!);
        }

        private static HttpRequestMessage Clone(HttpRequestMessage req)
        {
            var clone = new HttpRequestMessage(req.Method, req.RequestUri) { Version = req.Version };

            foreach (var h in req.Headers) clone.Headers.TryAddWithoutValidation(h.Key, h.Value);

            if (req.Options.TryGetValue(BodyKey, out string? body))
            {
                req.Options.TryGetValue(MediaTypeKey, out string? mt);
                clone.Content = new StringContent(body ?? string.Empty, Encoding.UTF8, mt ?? "application/json");
                foreach (var h in req.Content!.Headers)
                {
                    if (h.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase)) continue;
                    clone.Content.Headers.TryAddWithoutValidation(h.Key, h.Value);
                }
            }
            else
            {
                clone.Content = req.Content;
            }

            return clone;
        }
    }
}
