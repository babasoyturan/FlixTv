namespace FlixTv.Clients.WebApp.ViewModels
{
    public class MoviesViewModel
    {
        public MoviesFilter Filter { get; set; } = new();
        public MoviesListPartialViewModel List { get; set; } = new();
    }
}
