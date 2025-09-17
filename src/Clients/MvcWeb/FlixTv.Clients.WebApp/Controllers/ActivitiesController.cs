using FlixTv.Clients.WebApp.Services.Abstractions;
using FlixTv.Clients.WebApp.ViewModels;
using FlixTv.Common.Models.ResponseModels.Comments;
using FlixTv.Common.Models.ResponseModels.Reviews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlixTv.Clients.WebApp.Controllers
{
    [Authorize(Roles = "User, Admin, Moderator")]
    public class ActivitiesController : Controller
    {
        private readonly ICommentsService _comments;
        private readonly IReviewsService _reviews;

        public ActivitiesController(ICommentsService comments, IReviewsService reviews)
        {
            _comments = comments;
            _reviews = reviews;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int cPage = 1, int cSize = 10, int rPage = 1, int rSize = 5)
        {
            var vm = new ActivitiesViewModel
            {
                CommentsPage = Math.Max(1, cPage),
                CommentsPageSize = Math.Max(1, cSize),
                ReviewsPage = Math.Max(1, rPage),
                ReviewsPageSize = Math.Max(1, rSize)
            };

            // Counts
            var cc = await _comments.GetMyCommentsCountAsync();
            var rc = await _reviews.GetMyReviewsCountAsync();
            if (cc.IsSuccess) vm.MyCommentsCount = cc.Data;
            if (rc.IsSuccess) vm.MyReviewsCount = rc.Data;

            // Lists (yalnız count > 0 olduqda)
            if (vm.MyCommentsCount > 0)
            {
                var list = await _comments.GetMyCommentsAsync(vm.CommentsPage, vm.CommentsPageSize, "createdDate");
                if (list.IsSuccess) vm.MyComments = list.Data ?? new List<GetCommentQueryResponse>();
            }
            if (vm.MyReviewsCount > 0)
            {
                var list = await _reviews.GetMyReviewsAsync(vm.ReviewsPage, vm.ReviewsPageSize, "createdDate");
                if (list.IsSuccess) vm.MyReviews = list.Data ?? new List<GetReviewQueryResponse>();
            }

            return View(vm);
        }

        // ——— Partial-lar (AJAX) ———
        [HttpGet]
        public async Task<IActionResult> MyComments(int page = 1, int pageSize = 10)
        {
            var count = await _comments.GetMyCommentsCountAsync();
            if (!count.IsSuccess) return StatusCode(500, "Could not load comments.");

            var list = await _comments.GetMyCommentsAsync(page, pageSize, "createdDate");
            if (!list.IsSuccess) return StatusCode(500, "Could not load comments.");

            var vm = new MyCommentsTablePartialViewModel
            {
                Comments = list.Data ?? new List<GetCommentQueryResponse>(),
                TotalCount = count.Data,
                Page = page,
                PageSize = pageSize
            };
            return PartialView("_MyCommentsTable", vm);
        }

        [HttpGet]
        public async Task<IActionResult> MyReviews(int page = 1, int pageSize = 5)
        {
            var count = await _reviews.GetMyReviewsCountAsync();
            if (!count.IsSuccess) return StatusCode(500, "Could not load reviews.");

            var list = await _reviews.GetMyReviewsAsync(page, pageSize, "createdDate");
            if (!list.IsSuccess) return StatusCode(500, "Could not load reviews.");

            var vm = new MyReviewsTablePartialViewModel
            {
                Reviews = list.Data ?? new List<GetReviewQueryResponse>(),
                TotalCount = count.Data,
                Page = page,
                PageSize = pageSize
            };
            return PartialView("_MyReviewsTable", vm);
        }
    }
}
