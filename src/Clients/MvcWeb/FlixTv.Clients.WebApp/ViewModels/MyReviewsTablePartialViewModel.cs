using FlixTv.Common.Models.ResponseModels.Reviews;

namespace FlixTv.Clients.WebApp.ViewModels
{
    public sealed class MyReviewsTablePartialViewModel
    {
        public IList<GetReviewQueryResponse> Reviews { get; set; } = new List<GetReviewQueryResponse>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)Math.Max(0, TotalCount) / Math.Max(1, PageSize));
    }
}
