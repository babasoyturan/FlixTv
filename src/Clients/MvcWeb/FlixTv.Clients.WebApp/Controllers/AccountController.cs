using FlixTv.Clients.WebApp.Filters;
using FlixTv.Clients.WebApp.Services.Abstractions;
using FlixTv.Common.Models.RequestModels.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace FlixTv.Clients.WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService authService;

        public AccountController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpGet, AllowAnonymous, RedirectAuthenticated]
        public IActionResult Signin(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }


        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> Signin(LoginCommandRequest request, string? returnUrl)
        {
            if (!ModelState.IsValid)
                return View(request);

            var apiResult = await authService.LoginAsync(request, HttpContext);

            if (!apiResult.IsSuccess)
            {
                foreach (var error in apiResult.Errors)
                    ModelState.AddModelError(string.Empty, error);
                return View(request);
            }

            if (!string.IsNullOrWhiteSpace(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }


        [HttpGet, AllowAnonymous, RedirectAuthenticated]
        public IActionResult Signup()
        {
            return View();
        }


        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> Signup(RegisterCommandRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            request.ClientUri = $"{Request.Scheme}://{Request.Host}/Account/EmailConfirmation";

            var apiResult = await authService.RegisterAsync(request);

            if (!apiResult.IsSuccess)
            {
                foreach (var error in apiResult.Errors)
                    ModelState.AddModelError(string.Empty, error);
                return View(request);
            }

            TempData["Status"] = "success";
            TempData["Message"] = "Registration successful! Please check your email to confirm your account.";
            TempData["ReturnUrl"] = Url.Action("Signin", "Account");
            TempData["ButtonText"] = "Go to Sign In";

            return RedirectToAction("Index", "Info");
        }


        [HttpGet, AllowAnonymous, RedirectAuthenticated]
        public async Task<IActionResult> EmailConfirmation([FromQuery] string token, [FromQuery] string email)
        {
            var clientUri = "https://localhost:5000/api/Auth/EmailConfirmation";

            var param = new Dictionary<string, string>
            {
                { "token", token },
                { "email", email }
            };

            var callback = QueryHelpers.AddQueryString(clientUri, param);

            var apiResult = await authService.ConfirmEmailAsync(callback);

            if (apiResult.IsSuccess)
            {
                TempData["Status"] = "success";
                TempData["Message"] = apiResult.Data;
                TempData["ReturnUrl"] = Url.Action("Signin", "Account");
                TempData["ButtonText"] = "Go to Sign In";

                return RedirectToAction("Index", "Info");
            }

            TempData["Status"] = "fail";
            TempData["Message"] = apiResult.Errors[0];
            TempData["ReturnUrl"] = Url.Action("Signin", "Account");
            TempData["ButtonText"] = "Go to Sign In";

            return RedirectToAction("Index", "Info");
        }


        [HttpGet, AllowAnonymous, RedirectAuthenticated]
        public IActionResult ForgotPassword()
        {
            return View();
        }


        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordCommandRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            request.ClientUri = $"{Request.Scheme}://{Request.Host}/Account/ResetPassword";

            var apiResult = await authService.ForgotPasswordAsync(request);

            if (!apiResult.IsSuccess)
            {
                foreach (var error in apiResult.Errors)
                    ModelState.AddModelError(string.Empty, error);
                return View(request);
            }

            TempData["Status"] = "info";
            TempData["Message"] = apiResult.Data;
            TempData["ReturnUrl"] = Url.Action("Signin", "Account");
            TempData["ButtonText"] = "Go Back";

            return RedirectToAction("Index", "Info");
        }


        [HttpGet, AllowAnonymous, RedirectAuthenticated]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult ResetPassword([FromQuery] string token, [FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(email))
                return RedirectToAction("Index", "Home");

            Response.Headers["X-Robots-Tag"] = "noindex, nofollow";

            ViewBag.Token = token;
            ViewBag.Email = email;

            return View();
        }


        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordCommandRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            var apiResult = await authService.ResetPasswordAsync(request);

            if (!apiResult.IsSuccess)
            {
                TempData["Status"] = "fail";
                TempData["Message"] = apiResult.Errors[0];
            }
            else
            {
                TempData["Status"] = "success";
                TempData["Message"] = apiResult.Data;
            }

            TempData["ReturnUrl"] = Url.Action("Signin", "Account");
            TempData["ButtonText"] = "Go To Sigin";

            return RedirectToAction("Index", "Info");
        }
    }
}
