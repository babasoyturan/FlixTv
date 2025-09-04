using FlixTv.Common.Models.ResponseModels.Movies;

namespace FlixTv.Clients.WebApp.ViewModels
{
    public class WatchViewModel
    {
        public GetMovieQueryResponse Movie { get; set; }

        public IList<GetAllMoviesQueryResponse> SimiliarMovies { get; set; }
    }
}
