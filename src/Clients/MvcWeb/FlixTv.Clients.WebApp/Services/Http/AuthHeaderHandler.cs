using System.Net.Http.Headers;

namespace FlixTv.Clients.WebApp.Services.Http
{
    public sealed class AuthHeaderHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _ctx;
        private readonly IPerRequestTokenStore _store;

        public AuthHeaderHandler(IHttpContextAccessor ctx, IPerRequestTokenStore store)
        {
            _ctx = ctx; _store = store;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            if (request.Headers.Authorization is null)
            {
                // 1) Request-ölçülü store-dan götür
                var token = _store.AccessToken;

                // 2) Store boşdursa, ilk dəfə cookie-dən oxuyub store-a yaz
                if (string.IsNullOrWhiteSpace(token))
                {
                    token = _ctx.HttpContext?.Request.Cookies["flix_access_token"];
                    if (!string.IsNullOrWhiteSpace(token))
                        _store.AccessToken = token;
                }

                if (!string.IsNullOrWhiteSpace(token))
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return base.SendAsync(request, ct);
        }
    }
}
