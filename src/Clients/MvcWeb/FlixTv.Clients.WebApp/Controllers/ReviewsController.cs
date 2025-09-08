using FlixTv.Clients.WebApp.Services.Abstractions;
using FlixTv.Common.Models.RequestModels.Reviews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlixTv.Clients.WebApp.Controllers
{
    [Authorize(Roles = "User, Admin, Moderator")]
    public class ReviewsController : Controller
    {
        private readonly IReviewsService _reviews;

        public ReviewsController(IReviewsService reviews) => _reviews = reviews;


        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            int movieId,
            string title,
            string message,
            byte rating,
            CancellationToken ct)
        {
            var req = new CreateReviewCommandRequest
            {
                MovieId = movieId,
                Title = title?.Trim() ?? string.Empty,
                Message = message?.Trim() ?? string.Empty,
                RatingPoint = rating
            };

            var res = await _reviews.CreateReviewAsync(req, ct);
            if (!res.IsSuccess)
                return BadRequest(new { errors = res.Errors });

            return Ok(new { ok = true, message = res.Data });
        }


        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(
            int id, 
            CancellationToken ct)
        {
            var res = await _reviews.DeleteReviewAsync(id, ct);
            if (!res.IsSuccess)
                return StatusCode((int)res.StatusCode, new { success = false, errors = res.Errors });

            return Ok(new { success = true });
        }
    }
}
