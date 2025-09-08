using FlixTv.Common.Models.ResponseModels.Reviews;

namespace FlixTv.Clients.WebApp.ViewModels
{
    public class MovieReviewsPartialViewModel
    {
        public int MovieId { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / Math.Max(1, PageSize));
        public IList<GetReviewQueryResponse> Reviews { get; set; } = new List<GetReviewQueryResponse>();
    }
}
