using FlixTv.Clients.WebApp.Services.Http;
using FlixTv.Common.Models.RequestModels.Comments;
using FlixTv.Common.Models.ResponseModels.Comments;

namespace FlixTv.Clients.WebApp.Services.Abstractions
{
    public interface ICommentsService
    {
        Task<ApiResult<IList<GetCommentQueryResponse>>> GetMovieCommentsAsync(
            int movieId,
            int currentPage,
            int pageSize,
            string? orderBy = "createdDate",
            CancellationToken ct = default);

        Task<ApiResult<int>> GetMovieCommentsCountAsync(
            int movieId,
            CancellationToken ct = default);

        Task<ApiResult<GetCommentQueryResponse>> ReactAsync(
            int commentId, 
            string action, 
            CancellationToken ct = default);

        Task<ApiResult<string>> CreateCommentAsync(
            CreateCommentCommandRequest request, 
            CancellationToken ct = default);

        Task<ApiResult<bool>> DeleteCommentAsync(
            int commentId, 
            CancellationToken ct = default);
    }
}
