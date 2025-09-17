using FlixTv.Clients.WebApp.Services.Http;
using FlixTv.Common.Models.RequestModels.Reviews;
using FlixTv.Common.Models.ResponseModels.Reviews;

namespace FlixTv.Clients.WebApp.Services.Abstractions
{
    public interface IReviewsService
    {
        Task<ApiResult<IList<GetReviewQueryResponse>>> GetMovieReviewsAsync(
            int movieId,
            int currentPage,
            int pageSize,
            string? orderBy = "createdDate",
            CancellationToken ct = default);

        Task<ApiResult<int>> GetMovieReviewsCountAsync(
            int movieId,
            CancellationToken ct = default);

        Task<ApiResult<IList<GetReviewQueryResponse>>> GetMyReviewsAsync(
            int currentPage, 
            int pageSize, 
            string? orderBy = "createdDate", 
            CancellationToken ct = default);

        Task<ApiResult<int>> GetMyReviewsCountAsync(
            CancellationToken ct = default);

        Task<ApiResult<int>> GetMovieUserReviewCountAsync(
            int movieId,
            int authorId,
            CancellationToken ct = default);

        Task<ApiResult<GetReviewQueryResponse>> GetReviewAsync(
            int reviewId,
            CancellationToken ct = default);

        Task<ApiResult<string>> CreateReviewAsync(
            CreateReviewCommandRequest request,
            CancellationToken ct = default);

        Task<ApiResult<bool>> DeleteReviewAsync(
            int reviewId, 
            CancellationToken ct = default);
    }
}
