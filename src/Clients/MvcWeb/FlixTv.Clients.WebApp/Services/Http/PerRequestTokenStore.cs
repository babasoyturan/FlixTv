namespace FlixTv.Clients.WebApp.Services.Http
{
    public sealed class PerRequestTokenStore : IPerRequestTokenStore
    {
        private readonly IHttpContextAccessor _ctx;
        private const string AccessKey = "__flix_access_token";
        private const string RefreshKey = "__flix_refresh_token";

        public PerRequestTokenStore(IHttpContextAccessor ctx) => _ctx = ctx;

        private IDictionary<object, object?> Items =>
            _ctx.HttpContext?.Items ?? throw new InvalidOperationException("No HttpContext");

        public string? AccessToken
        {
            get => Items.TryGetValue(AccessKey, out var v) ? v as string : null;
            set => Items[AccessKey] = value;
        }

        public string? RefreshToken
        {
            get => Items.TryGetValue(RefreshKey, out var v) ? v as string : null;
            set => Items[RefreshKey] = value;
        }
    }
}
