using FlixTv.Common.Models.ResponseModels.Movies;

namespace FlixTv.Clients.WebApp.ViewModels
{
    public class HomeViewModel
    {
        public string RecommendedTitlePart1 { get; set; } = "Best Movies";
        public string RecommendedTitlePart2 { get; set; } = " of FlixTV";
        public IList<GetAllMoviesQueryResponse> RecommendedMovies { get; set; }
        public IList<RowViewModel> Rows { get; set; }
    }
}
