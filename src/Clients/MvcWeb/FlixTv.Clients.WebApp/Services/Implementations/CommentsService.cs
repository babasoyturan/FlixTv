using FlixTv.Clients.WebApp.Services.Abstractions;
using FlixTv.Clients.WebApp.Services.Http;
using FlixTv.Common.Models.RequestModels.Comments;
using FlixTv.Common.Models.ResponseModels.Comments;

namespace FlixTv.Clients.WebApp.Services.Implementations
{
    public class CommentsService : BaseApiClient, ICommentsService
    {
        private const string GetAllCommentsEndpoint = "Comments/GetAllComments";
        private const string GetCommentsCountEndpoint = "Comments/GetCommentsCount";
        private const string GetCommentEndpoint = "Comments/GetComment";
        private const string IncLikeEndpoint = "Comments/IncrementCommentLike";
        private const string DecLikeEndpoint = "Comments/DecrementCommentLike";
        private const string IncDislikeEndpoint = "Comments/IncrementCommentDislike";
        private const string DecDislikeEndpoint = "Comments/DecrementCommentDislike";
        private const string CreateCommentEndpoint = "Comments/CreateComment";
        private const string DeleteCommentEndpoint = "Comments/DeleteComment";
        private const string GetMyCommentsEndpoint = "Comments/GetMyComments";
        private const string GetMyCommentsCountEndpoint = "Comments/GetMyCommentsCount";

        public CommentsService(IHttpClientFactory factory)
            : base(factory.CreateClient("flix-api")) { }

        public Task<ApiResult<IList<GetCommentQueryResponse>>> GetMovieCommentsAsync(
            int movieId,
            int currentPage,
            int pageSize,
            string? orderBy = "createdDate",
            CancellationToken ct = default)
        {
            if (movieId <= 0)
                return Task.FromResult(ApiResult<IList<GetCommentQueryResponse>>
                    .Fail(new[] { "Invalid movie id." }, System.Net.HttpStatusCode.BadRequest));

            if (currentPage <= 0 || pageSize <= 0)
                return Task.FromResult(ApiResult<IList<GetCommentQueryResponse>>
                    .Fail(new[] { "Invalid pagination. currentPage and pageSize must be > 0." },
                          System.Net.HttpStatusCode.BadRequest));

            var url = $"{GetAllCommentsEndpoint}?movieId={movieId}&currentPage={currentPage}&pageSize={pageSize}";
            if (!string.IsNullOrWhiteSpace(orderBy))
                url += $"&orderBy={Uri.EscapeDataString(orderBy)}"; // createdDate | likeCount | dislikeCount

            return GetAsync<IList<GetCommentQueryResponse>>(url, ct);
        }

        public Task<ApiResult<int>> GetMovieCommentsCountAsync(
            int movieId,
            CancellationToken ct = default)
        {
            if (movieId <= 0)
                return Task.FromResult(ApiResult<int>
                    .Fail(new[] { "Invalid movie id." }, System.Net.HttpStatusCode.BadRequest));

            var url = $"{GetCommentsCountEndpoint}?movieId={movieId}";
            return GetAsync<int>(url, ct);
        }

        public Task<ApiResult<IList<GetCommentQueryResponse>>> GetMyCommentsAsync(
            int currentPage, 
            int pageSize, 
            string? orderBy = "createdDate", 
            CancellationToken ct = default)
        {
            if (currentPage <= 0 || pageSize <= 0)
                return Task.FromResult(ApiResult<IList<GetCommentQueryResponse>>
                    .Fail(new[] { "Invalid pagination." }, System.Net.HttpStatusCode.BadRequest));

            var url = $"{GetMyCommentsEndpoint}?currentPage={currentPage}&pageSize={pageSize}";
            if (!string.IsNullOrWhiteSpace(orderBy)) url += $"&orderBy={Uri.EscapeDataString(orderBy)}";
            return GetAsync<IList<GetCommentQueryResponse>>(url, ct);
        }

        public Task<ApiResult<int>> GetMyCommentsCountAsync(CancellationToken ct = default)
            => GetAsync<int>(GetMyCommentsCountEndpoint, ct);

        public async Task<ApiResult<GetCommentQueryResponse>> ReactAsync(
            int commentId,
            string action,
            CancellationToken ct = default)
        {
            if (commentId <= 0)
                return ApiResult<GetCommentQueryResponse>.Fail(new[] { "Invalid comment id." }, System.Net.HttpStatusCode.BadRequest);

            var url = action switch
            {
                "like" => $"{IncLikeEndpoint}?commentId={commentId}",
                "unlike" => $"{DecLikeEndpoint}?commentId={commentId}",
                "dislike" => $"{IncDislikeEndpoint}?commentId={commentId}",
                "undislike" => $"{DecDislikeEndpoint}?commentId={commentId}",
                _ => null
            };
            if (url is null)
                return ApiResult<GetCommentQueryResponse>.Fail(new[] { "Invalid action." }, System.Net.HttpStatusCode.BadRequest);

            // POST – dəyişiklik et
            var post = await PostAsync(url, null, ct);
            if (!post.IsSuccess) // <-- Success yox, IsSuccess
                return ApiResult<GetCommentQueryResponse>.Fail(post.Errors, post.StatusCode);

            // GET – ən son dəqiq state-i qaytar
            return await GetAsync<GetCommentQueryResponse>($"{GetCommentEndpoint}/{commentId}", ct);
        }

        public Task<ApiResult<string>> CreateCommentAsync(
            CreateCommentCommandRequest request,
            CancellationToken ct = default)
        {
            if (request is null || request.MovieId <= 0 || string.IsNullOrWhiteSpace(request.Message))
                return Task.FromResult(ApiResult<string>.Fail(new[] { "Invalid input" }, System.Net.HttpStatusCode.BadRequest));

            return PostJsonAsync<CreateCommentCommandRequest, string>(CreateCommentEndpoint, request, ct);
        }


        public Task<ApiResult<bool>> DeleteCommentAsync(
            int commentId, 
            CancellationToken ct = default)
        {
            if (commentId <= 0)
                return Task.FromResult(ApiResult<bool>
                    .Fail(new[] { "Invalid comment id." }, System.Net.HttpStatusCode.BadRequest));

            return PostAsync($"{DeleteCommentEndpoint}/{commentId}", null, ct);
        }
    }
}
