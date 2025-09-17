using FlixTv.Common.Models.ResponseModels.Comments;

namespace FlixTv.Clients.WebApp.ViewModels
{
    public sealed class MyCommentsTablePartialViewModel
    {
        public IList<GetCommentQueryResponse> Comments { get; set; } = new List<GetCommentQueryResponse>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)Math.Max(0, TotalCount) / Math.Max(1, PageSize));
    }
}
