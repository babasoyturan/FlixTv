using FlixTv.Clients.WebApp.Services.Abstractions;
using FlixTv.Clients.WebApp.ViewModels;
using FlixTv.Common.Models.RequestModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FlixTv.Clients.WebApp.Controllers
{
    [Authorize(Roles = "User, Moderator, Admin")]
    public class ProfileController : Controller
    {
        private readonly IUsersService _users;
        private readonly IAuthService _auth;

        public ProfileController(IUsersService users, IAuthService auth)
        { _users = users; _auth = auth; }

        [HttpGet]
        public IActionResult Index()
        {
            var vm = new ProfileViewModel
            {
                Id = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 0,
                Name = User.FindFirst("name")?.Value,
                Surname = User.FindFirst("surname")?.Value,
                Email = User.FindFirst(ClaimTypes.Email)?.Value
            };
            return View(vm);
        }

        // AJAX: Profile save
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([FromBody] UpdateUserCommandRequest req, CancellationToken ct)
        {
            // original email-i clientdən istəməyə ehtiyac yoxdur
            var originalEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;

            var api = await _users.UpdateAsync(req, ct);
            if (!api.IsSuccess)
                return StatusCode((int)api.StatusCode, api.Errors.FirstOrDefault() ?? "Update failed");

            var emailChanged = !string.IsNullOrWhiteSpace(req.Email) &&
                               !string.Equals(req.Email, originalEmail, StringComparison.OrdinalIgnoreCase);

            if (emailChanged)
            {
                await _auth.LogoutAsync(HttpContext); // backend refresh tokenləri də sıfırlayıb
                return Ok(new { redirect = Url.Action("Signin", "Account") });
            }

            await _auth.UpdateCookieClaimsAsync(HttpContext, req.Name, req.Surname, null);
            return Ok(new { ok = true, emailChanged = false, name = req.Name, surname = req.Surname });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommandRequest req, CancellationToken ct)
        {
            var api = await _users.ChangePasswordAsync(req, ct);
            if (!api.IsSuccess)
                return StatusCode((int)api.StatusCode, api.Errors.FirstOrDefault() ?? "Change password failed");

            return Ok(new { ok = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _auth.LogoutAsync(HttpContext);
            return Ok(new { ok = true });
        }
    }
}
