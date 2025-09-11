using FlixTv.Clients.WebApp.Services.Abstractions;
using FlixTv.Clients.WebApp.Services.Http;
using FlixTv.Common.Models.RequestModels.ViewDatas;

namespace FlixTv.Clients.WebApp.Services.Implementations
{
    public class ViewDatasService : BaseApiClient, IViewDatasService
    {
        private const string CreateEndpoint = "ViewDatas/CreateViewData";

        public ViewDatasService(IHttpClientFactory factory)
            : base(factory.CreateClient("flix-api")) { }

        public async Task<ApiResult<string>> CreateAsync(
            CreateViewDataCommandRequest request,
            CancellationToken ct = default)
        {
            if (request is null
                || request.MovieId <= 0
                || request.WatchedSeconds <= 120
                || request.MaxPositionSeconds < request.LastPositionSeconds)
                return ApiResult<string>.Fail(new[] { "Invalid input." }, System.Net.HttpStatusCode.BadRequest);

            return await PostJsonAsync<CreateViewDataCommandRequest, string>(CreateEndpoint, request, ct);
        }
    }
}
