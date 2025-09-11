using FlixTv.Clients.WebApp.Services.Http;
using FlixTv.Common.Models.RequestModels.ViewDatas;

namespace FlixTv.Clients.WebApp.Services.Abstractions
{
    public interface IViewDatasService
    {
        Task<ApiResult<string>> CreateAsync(
            CreateViewDataCommandRequest request,
            CancellationToken ct = default);
    }
}
