using FlixTv.Clients.WebApp.Services.Http;
using FlixTv.Common.Models.ResponseModels.Movies;

namespace FlixTv.Clients.WebApp.Services.Abstractions
{
    public interface IMoviesService
    {
        Task<ApiResult<IList<GetRowModelsQueryResponse>>> GetRowModelsAsync(
            int count = 12,
            CancellationToken ct = default);

        Task<ApiResult<IList<GetAllMoviesQueryResponse>>> GetMoviesForRowAsync(
            GetRowModelsQueryResponse row,
            int count = 12,
            CancellationToken ct = default);
    }
}
