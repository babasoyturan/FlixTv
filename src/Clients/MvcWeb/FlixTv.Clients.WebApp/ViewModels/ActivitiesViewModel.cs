using FlixTv.Common.Models.ResponseModels.Comments;
using FlixTv.Common.Models.ResponseModels.Reviews;

namespace FlixTv.Clients.WebApp.ViewModels
{
    public class ActivitiesViewModel
    {
        // Comments
        public IList<GetCommentQueryResponse> MyComments { get; set; } = new List<GetCommentQueryResponse>();
        public int MyCommentsCount { get; set; }
        public int CommentsPage { get; set; } = 1;
        public int CommentsPageSize { get; set; } = 10;
        public int CommentsTotalPages => (int)Math.Ceiling((double)Math.Max(0, MyCommentsCount) / Math.Max(1, CommentsPageSize));

        // Reviews
        public IList<GetReviewQueryResponse> MyReviews { get; set; } = new List<GetReviewQueryResponse>();
        public int MyReviewsCount { get; set; }
        public int ReviewsPage { get; set; } = 1;
        public int ReviewsPageSize { get; set; } = 5;
        public int ReviewsTotalPages => (int)Math.Ceiling((double)Math.Max(0, MyReviewsCount) / Math.Max(1, ReviewsPageSize));
    }
}
