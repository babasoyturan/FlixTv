using FlixTv.Common.Models.ResponseModels.Movies;

namespace FlixTv.Clients.WebApp.ViewModels
{
    public class HomeViewModel
    {
        public IList<GetAllMoviesQueryResponse> RecommendedMovies { get; set; }
        public IList<RowViewModel> Rows { get; set; }
    }
}
