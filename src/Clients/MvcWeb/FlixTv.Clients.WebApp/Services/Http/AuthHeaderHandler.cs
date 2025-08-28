using System.Net.Http.Headers;

namespace FlixTv.Clients.WebApp.Services.Http
{
    public sealed class AuthHeaderHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _ctx;

        public AuthHeaderHandler(IHttpContextAccessor ctx) => _ctx = ctx;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            var token = _ctx.HttpContext?.Request.Cookies["flix_access_token"];
            if (!string.IsNullOrWhiteSpace(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return base.SendAsync(request, ct);
        }
    }
}
