namespace FlixTv.Clients.WebApp.Services.Http
{
    public interface IPerRequestTokenStore
    {
        string? AccessToken { get; set; }
        string? RefreshToken { get; set; }
    }
}
