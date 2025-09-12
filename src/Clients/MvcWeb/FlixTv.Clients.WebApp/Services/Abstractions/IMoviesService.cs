using FlixTv.Clients.WebApp.Services.Http;
using FlixTv.Clients.WebApp.ViewModels;
using FlixTv.Common.Models.ResponseModels.Movies;
using System.Threading.Tasks;

namespace FlixTv.Clients.WebApp.Services.Abstractions
{
    public interface IMoviesService
    {
        Task<ApiResult<GetMovieQueryResponse>> GetMovieAsync(
            int id,
            CancellationToken ct = default);

        Task<ApiResult<IList<GetAllMoviesQueryResponse>>> GetRelatedMoviesAsync(
            int movieId,
            CancellationToken ct = default);

        Task<ApiResult<IList<GetRowModelsQueryResponse>>> GetRowModelsAsync(
            int count = 12,
            CancellationToken ct = default);

        Task<ApiResult<IList<GetAllMoviesQueryResponse>>> GetMoviesByUserCompatibilityAsync(
            int count = 12,
            CancellationToken ct = default);

        Task<ApiResult<IList<GetAllMoviesQueryResponse>>> GetMoviesForRowAsync(
            GetRowModelsQueryResponse row,
            int count = 12,
            CancellationToken ct = default);

        Task<ApiResult<IList<GetAllMoviesQueryResponse>>> GetLatestMoviesAsync(
            int pool = 50,
            CancellationToken ct = default);

        Task<ApiResult<IList<GetAllMoviesQueryResponse>>> GetAllMoviesAsync(
            MoviesFilter filter,
            CancellationToken ct = default);

        Task<ApiResult<IList<GetAllMoviesQueryResponse>>> GetUnfinishedMoviesAsync(
            CancellationToken ct = default);

        Task<ApiResult<int>> GetMoviesCountAsync(
            MoviesFilter filter, 
            CancellationToken ct = default);
    }
}
