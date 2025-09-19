using FlixTv.Clients.WebApp.Services.Abstractions;
using FlixTv.Clients.WebApp.Services.Http;
using FlixTv.Common.Models.RequestModels.ViewDatas;
using FlixTv.Common.Models.ResponseModels.ViewData;

namespace FlixTv.Clients.WebApp.Services.Implementations
{
    public class ViewDatasService : BaseApiClient, IViewDatasService
    {
        private const string CreateEndpoint = "ViewDatas/CreateViewData";
        private const string GetMyViewDatasEndpoint = "ViewDatas/GetMyViewDatas";
        private const string GetMyViewDatasCountEndpoint = "ViewDatas/GetMyViewDatasCount";


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

        public async Task<ApiResult<IList<GetViewDataQueryResponse>>> GetMyViewDatasAsync(
            int page = 1, 
            int pageSize = 10, 
            string? orderBy = null, 
            CancellationToken ct = default)
        {
            var url = $"{GetMyViewDatasEndpoint}?currentPage={page}&pageSize={pageSize}";
            if (!string.IsNullOrWhiteSpace(orderBy)) url += $"&orderBy={Uri.EscapeDataString(orderBy)}";
            return await GetAsync<IList<GetViewDataQueryResponse>>(url, ct);
        }

        public async Task<ApiResult<int>> GetMyViewDatasCountAsync(
            CancellationToken ct = default)
        {
            return await GetAsync<int>(GetMyViewDatasCountEndpoint, ct);
        }
    }
}
