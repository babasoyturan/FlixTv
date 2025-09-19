using FlixTv.Clients.WebApp.Services.Http;
using FlixTv.Common.Models.RequestModels.ViewDatas;
using FlixTv.Common.Models.ResponseModels.ViewData;

namespace FlixTv.Clients.WebApp.Services.Abstractions
{
    public interface IViewDatasService
    {
        Task<ApiResult<string>> CreateAsync(
            CreateViewDataCommandRequest request,
            CancellationToken ct = default);

        Task<ApiResult<IList<GetViewDataQueryResponse>>> GetMyViewDatasAsync
            (int page = 1, 
            int pageSize = 10, 
            string? orderBy = null, 
            CancellationToken ct = default);

        Task<ApiResult<int>> GetMyViewDatasCountAsync(
            CancellationToken ct = default);
    }
}
