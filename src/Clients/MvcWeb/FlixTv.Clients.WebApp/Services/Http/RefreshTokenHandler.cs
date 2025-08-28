using FlixTv.Common.Models.ResponseModels.Auth;
using System.Net;

namespace FlixTv.Clients.WebApp.Services.Http
{
    public sealed class RefreshTokenHandler : DelegatingHandler
    {
        private static readonly SemaphoreSlim _gate = new(1, 1);
        private readonly IHttpClientFactory _httpFactory;
        private readonly IHttpContextAccessor _ctx;

        public RefreshTokenHandler(IHttpClientFactory httpFactory, IHttpContextAccessor ctx)
        {
            _httpFactory = httpFactory;
            _ctx = ctx;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            // İkinci dəfə retry olmasın deyə işarə
            var alreadyRetried = request.Headers.Contains("X-Retried-After-Refresh");

            var response = await base.SendAsync(request, ct);
            if (response.StatusCode != HttpStatusCode.Unauthorized || alreadyRetried)
                return response;

            await _gate.WaitAsync(ct);
            try
            {
                // Başqa thread refresh edibsə təkrar lazım deyil
                var token = _ctx.HttpContext?.Request.Cookies["flix_access_token"];
                if (!string.IsNullOrWhiteSpace(token))
                {
                    // yenidən göndər
                    request.Headers.Remove("Authorization");
                    request.Headers.Add("X-Retried-After-Refresh", "1");
                    return await base.SendAsync(Clone(request), ct);
                }

                // refresh cəhdi
                var refresh = _ctx.HttpContext?.Request.Cookies["flix_refresh_token"];
                if (string.IsNullOrWhiteSpace(refresh))
                    return response;

                var authClient = _httpFactory.CreateClient("flix-auth");
                var refreshResp = await authClient.PostAsJsonAsync("api/Auth/RefreshToken", new { RefreshToken = refresh }, ct);
                if (!refreshResp.IsSuccessStatusCode) return response;

                var data = await refreshResp.Content.ReadFromJsonAsync<LoginCommandResponse>(cancellationToken: ct);
                if (data is null) return response;

                SaveTokensToCookies(_ctx.HttpContext!, data);

                // təkrar orijinal sorğu
                request.Headers.Remove("Authorization");
                request.Headers.Add("X-Retried-After-Refresh", "1");
                return await base.SendAsync(Clone(request), ct);
            }
            finally
            {
                _gate.Release();
            }
        }

        private static HttpRequestMessage Clone(HttpRequestMessage req)
        {
            var clone = new HttpRequestMessage(req.Method, req.RequestUri)
            {
                Content = req.Content,
                Version = req.Version
            };
            foreach (var h in req.Headers) clone.Headers.TryAddWithoutValidation(h.Key, h.Value);
            return clone;
        }

        private static void SaveTokensToCookies(HttpContext ctx, LoginCommandResponse data)
        {
            var opts = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = data.Expiration
            };
            ctx.Response.Cookies.Append("flix_access_token", data.Token, opts);
            ctx.Response.Cookies.Append("flix_refresh_token", data.RefreshToken, opts);
        }
    }
}
