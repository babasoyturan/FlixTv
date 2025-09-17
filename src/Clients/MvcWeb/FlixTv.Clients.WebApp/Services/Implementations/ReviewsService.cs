using FlixTv.Clients.WebApp.Services.Abstractions;
using FlixTv.Clients.WebApp.Services.Http;
using FlixTv.Common.Models.RequestModels.Reviews;
using FlixTv.Common.Models.ResponseModels.Reviews;

namespace FlixTv.Clients.WebApp.Services.Implementations
{
    public sealed class ReviewsService : BaseApiClient, IReviewsService
    {
        private const string GetAllReviewsEndpoint = "Reviews/GetAllReviews";
        private const string GetReviewsCountEndpoint = "Reviews/GetReviewsCount";
        private const string GetReviewEndpoint = "Reviews/GetReview";
        private const string CreateReviewEndpoint = "Reviews/CreateReview";
        private const string DeleteReviewEndpoint = "Reviews/DeleteReview";
        private const string GetMyReviewsEndpoint = "Reviews/GetMyReviews";
        private const string GetMyReviewsCountEndpoint = "Reviews/GetMyReviewsCount";

        public ReviewsService(IHttpClientFactory factory)
            : base(factory.CreateClient("flix-api")) { }

        public Task<ApiResult<IList<GetReviewQueryResponse>>> GetMovieReviewsAsync(
            int movieId,
            int currentPage,
            int pageSize,
            string? orderBy = "createdDate",
            CancellationToken ct = default)
        {
            if (movieId <= 0)
                return Task.FromResult(ApiResult<IList<GetReviewQueryResponse>>
                    .Fail(new[] { "Invalid movie id." }, System.Net.HttpStatusCode.BadRequest));

            if (currentPage <= 0 || pageSize <= 0)
                return Task.FromResult(ApiResult<IList<GetReviewQueryResponse>>
                    .Fail(new[] { "Invalid pagination. currentPage and pageSize must be > 0." },
                          System.Net.HttpStatusCode.BadRequest));

            var url = $"{GetAllReviewsEndpoint}?movieId={movieId}&currentPage={currentPage}&pageSize={pageSize}";
            if (!string.IsNullOrWhiteSpace(orderBy))
                url += $"&orderBy={Uri.EscapeDataString(orderBy)}";

            return GetAsync<IList<GetReviewQueryResponse>>(url, ct);
        }

        public Task<ApiResult<int>> GetMovieReviewsCountAsync(
            int movieId,
            CancellationToken ct = default)
        {
            if (movieId <= 0)
                return Task.FromResult(ApiResult<int>
                    .Fail(new[] { "Invalid movie id." }, System.Net.HttpStatusCode.BadRequest));

            var url = $"{GetReviewsCountEndpoint}?movieId={movieId}";
            return GetAsync<int>(url, ct);
        }

        public Task<ApiResult<IList<GetReviewQueryResponse>>> GetMyReviewsAsync(
            int currentPage, 
            int pageSize, 
            string? orderBy = "createdDate", 
            CancellationToken ct = default)
        {
            if (currentPage <= 0 || pageSize <= 0)
                return Task.FromResult(ApiResult<IList<GetReviewQueryResponse>>
                    .Fail(new[] { "Invalid pagination." }, System.Net.HttpStatusCode.BadRequest));

            var url = $"{GetMyReviewsEndpoint}?currentPage={currentPage}&pageSize={pageSize}";
            if (!string.IsNullOrWhiteSpace(orderBy)) url += $"&orderBy={Uri.EscapeDataString(orderBy)}";
            return GetAsync<IList<GetReviewQueryResponse>>(url, ct);
        }

        public Task<ApiResult<int>> GetMyReviewsCountAsync(
            CancellationToken ct = default)
            => GetAsync<int>(GetMyReviewsCountEndpoint, ct);

        public Task<ApiResult<int>> GetMovieUserReviewCountAsync(
            int movieId,
            int authorId,
            CancellationToken ct = default)
        {
            if (movieId <= 0 || authorId <= 0)
                return Task.FromResult(ApiResult<int>
                    .Fail(new[] { "Invalid movieId/authorId." }, System.Net.HttpStatusCode.BadRequest));

            var url = $"{GetReviewsCountEndpoint}?movieId={movieId}&authorId={authorId}";
            return GetAsync<int>(url, ct);
        }

        public Task<ApiResult<GetReviewQueryResponse>> GetReviewAsync(
            int reviewId,
            CancellationToken ct = default)
        {
            if (reviewId <= 0)
                return Task.FromResult(ApiResult<GetReviewQueryResponse>
                    .Fail(new[] { "Invalid review id." }, System.Net.HttpStatusCode.BadRequest));

            var url = $"{GetReviewEndpoint}/{reviewId}";
            return GetAsync<GetReviewQueryResponse>(url, ct);
        }

        public async Task<ApiResult<string>> CreateReviewAsync(
            CreateReviewCommandRequest request,
            CancellationToken ct = default)
        {
            if (request is null
                || request.MovieId <= 0
                || string.IsNullOrWhiteSpace(request.Title)
                || string.IsNullOrWhiteSpace(request.Message)
                || request.RatingPoint is < 1 or > 10)
            {
                return ApiResult<string>.Fail(new[] { "Invalid input." }, System.Net.HttpStatusCode.BadRequest);
            }

            return await PostJsonAsync<CreateReviewCommandRequest, string>(CreateReviewEndpoint, request, ct);
        }

        public Task<ApiResult<bool>> DeleteReviewAsync(
            int reviewId, 
            CancellationToken ct = default)
        {
            if (reviewId <= 0)
                return Task.FromResult(ApiResult<bool>
                    .Fail(new[] { "Invalid review id." }, System.Net.HttpStatusCode.BadRequest));

            return PostAsync($"{DeleteReviewEndpoint}/{reviewId}", null, ct);
        }
    }
}
