using FlixTv.Common.Models.ResponseModels.Comments;
using FlixTv.Common.Models.ResponseModels.Movies;
using FlixTv.Common.Models.ResponseModels.Reviews;

namespace FlixTv.Clients.WebApp.ViewModels
{
    public class WatchViewModel
    {
        public GetMovieQueryResponse Movie { get; set; }

        public IList<GetAllMoviesQueryResponse> SimiliarMovies { get; set; }

        public IList<GetCommentQueryResponse> Comments { get; set; }
        public int CommentsCount { get; set; }
        public int CommentsPage { get; set; } = 1;
        public int CommentsPageSize { get; set; } = 5;
        public int CommentsTotalPages => (int)Math.Ceiling((double)CommentsCount / Math.Max(1, CommentsPageSize));


        public IList<GetReviewQueryResponse> Reviews { get; set; } = new List<GetReviewQueryResponse>();
        public int ReviewsCount { get; set; }
        public int ReviewsPage { get; set; } = 1;
        public int ReviewsPageSize { get; set; } = 5;
        public int ReviewsTotalPages => (int)Math.Ceiling((double)ReviewsCount / Math.Max(1, ReviewsPageSize));
        public bool CanAddReview { get; set; } = false;
    }
}
