namespace FlixTv.Clients.WebApp.Services.Http
{
    public sealed class CorrelationIdHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            request.Headers.TryAddWithoutValidation("X-Correlation-Id", Guid.NewGuid().ToString());
            return base.SendAsync(request, ct);
        }
    }
}
