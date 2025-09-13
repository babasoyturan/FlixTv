using FlixTv.Clients.WebApp.Services.Http;
using FlixTv.Common.Models.ResponseModels.FavouriteMovies;

namespace FlixTv.Clients.WebApp.Services.Abstractions
{
    public interface IFavouriteMoviesService
    {
        Task<ApiResult<IList<GetFavouriteMovieQueryResponse>>> GetMyFavouriteMoviesAsync(
            int page, 
            int pageSize, 
            CancellationToken ct = default);

        Task<ApiResult<int>> GetMyFavouriteMoviesCountAsync(
            CancellationToken ct = default);

        Task<ApiResult<bool>> ToggleFavouriteMovieAsync(
            int movieId, 
            CancellationToken ct = default);
    }
}
