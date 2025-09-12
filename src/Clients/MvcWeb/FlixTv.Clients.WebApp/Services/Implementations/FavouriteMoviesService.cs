using FlixTv.Clients.WebApp.Services.Abstractions;
using FlixTv.Clients.WebApp.Services.Http;
using FlixTv.Common.Models.ResponseModels.FavouriteMovies;

namespace FlixTv.Clients.WebApp.Services.Implementations
{
    public class FavouriteMoviesService : BaseApiClient, IFavouriteMoviesService
    {
        private const string MyFavsEndpoint = "FavouriteMovies/GetMyFavouriteMovies";
        private const string MyFavsCountEndpoint = "FavouriteMovies/GetMyFavouriteMoviesCount";

        public FavouriteMoviesService(IHttpClientFactory factory)
            : base(factory.CreateClient("flix-api")) { }

        public Task<ApiResult<IList<GetFavouriteMovieQueryResponse>>> GetMyFavouriteMoviesAsync(
            int page, 
            int pageSize, 
            CancellationToken ct = default)
        {
            page = Math.Max(1, page);
            pageSize = Math.Max(1, pageSize);
            var url = $"{MyFavsEndpoint}?currentPage={page}&pageSize={pageSize}";
            return GetAsync<IList<GetFavouriteMovieQueryResponse>>(url, ct);
        }

        public Task<ApiResult<int>> GetMyFavouriteMoviesCountAsync(
            CancellationToken ct = default)
            => GetAsync<int>(MyFavsCountEndpoint, ct);
    }
}
