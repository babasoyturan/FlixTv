using FlixTv.Clients.WebApp.Services.Abstractions;
using FlixTv.Common.Models.RequestModels.Comments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlixTv.Clients.WebApp.Controllers
{
    [Authorize(Roles = "User, Admin, Moderator")]
    public class CommentsController : Controller
    {
        private readonly ICommentsService _comments;
        public CommentsController(ICommentsService comments) => _comments = comments;


        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int movieId, string text, CancellationToken ct)
        {
            var res = await _comments.CreateCommentAsync(new CreateCommentCommandRequest
            {
                MovieId = movieId,
                Message = text
            }, ct);

            if (!res.IsSuccess)
                return BadRequest(new { errors = res.Errors });

            return Ok(new { ok = true });
        }


        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var res = await _comments.DeleteCommentAsync(id, ct);
            if (!res.IsSuccess)
                return StatusCode((int)res.StatusCode, new { success = false, errors = res.Errors });

            return Ok(new { success = true });
        }


        [HttpPost]
        public async Task<IActionResult> React(int id, string action, CancellationToken ct)
        {
            var res = await _comments.ReactAsync(id, action, ct);
            if (!res.IsSuccess)
                return StatusCode((int)res.StatusCode, new { success = false, errors = res.Errors });

            var c = res.Data!;
            return Json(new
            {
                success = true,
                commentId = c.Id,
                likeCount = c.LikeCount,
                dislikeCount = c.DislikeCount,
                hasLiked = c.HasLiked,
                hasDisliked = c.HasDisliked
            });
        }
    }
}
