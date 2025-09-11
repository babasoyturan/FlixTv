using FlixTv.Clients.WebApp.Services.Abstractions;
using FlixTv.Common.Models.RequestModels.ViewDatas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlixTv.Clients.WebApp.Controllers
{
    [Authorize(Roles = "User, Moderator, Admin")]
    public class ViewDatasController : Controller
    {
        private readonly IViewDatasService service;
        public ViewDatasController(IViewDatasService service) => this.service = service;

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            int movieId, 
            int lastPositionSeconds,
            int maxPositionSeconds, 
            int watchedSeconds, CancellationToken ct)
        {
            if (movieId <= 0) return BadRequest("Invalid movieId.");

            var req = new CreateViewDataCommandRequest
            {
                MovieId = movieId,
                LastPositionSeconds = Math.Max(0, lastPositionSeconds),
                MaxPositionSeconds = Math.Max(Math.Max(0, maxPositionSeconds), Math.Max(0, lastPositionSeconds)),
                WatchedSeconds = Math.Max(0, watchedSeconds)
            };

            var res = await service.CreateAsync(req, ct);
            if (!res.IsSuccess)
                return StatusCode((int)res.StatusCode, res.Errors?.FirstOrDefault() ?? "Failed");

            return Ok(new { ok = true });
        }
    }
}
