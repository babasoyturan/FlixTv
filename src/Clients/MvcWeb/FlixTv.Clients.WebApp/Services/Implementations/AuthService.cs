using FlixTv.Clients.WebApp.Services.Abstractions;
using FlixTv.Clients.WebApp.Services.Http;
using FlixTv.Common.Models.RequestModels.Auth;
using FlixTv.Common.Models.ResponseModels.Auth;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FlixTv.Clients.WebApp.Services.Implementations
{
    public sealed class AuthService : BaseApiClient, IAuthService
    {
        public AuthService(IHttpClientFactory factory) : base(factory.CreateClient("flix-auth")) { }

        public async Task<ApiResult<LoginCommandResponse>> LoginAsync(LoginCommandRequest request, HttpContext ctx)
        {
            var result = await PostJsonAsync<LoginCommandRequest, LoginCommandResponse>("Auth/Login", request);
            if (!result.IsSuccess || result.Data is null) return result;

            // success: tokenləri saxla və MVC cookie-si ilə sign-in et
            SaveTokens(ctx, result.Data);
            var principal = BuildPrincipalFromJwt(result.Data.Token);
            await ctx.SignInAsync("Cookies", principal);

            return result;
        }

        public async Task<ApiResult<string>> RegisterAsync(RegisterCommandRequest request)
        {
            var result = await PostJsonAsync<RegisterCommandRequest, string>("Auth/Register", request);

            return result;
        }

        public async Task<ApiResult<string>> ForgotPasswordAsync(ForgotPasswordCommandRequest request)
        {
            var result = await PostJsonAsync<ForgotPasswordCommandRequest, string>("Auth/ForgotPassword", request);

            return result;
        }

        public async Task<ApiResult<string>> ResetPasswordAsync(ResetPasswordCommandRequest request)
        {
            var result = await PostJsonAsync<ResetPasswordCommandRequest, string>("Auth/ResetPassword", request);

            return result;
        }

        public async Task<ApiResult<string>> ConfirmEmailAsync(string url)
        {
            var result = await GetAsync<string>(url);

            return result;
        }

        public async Task LogoutAsync(HttpContext ctx)
        {
            ctx.Response.Cookies.Delete("flix_access_token");
            ctx.Response.Cookies.Delete("flix_refresh_token");
            await ctx.SignOutAsync("Cookies");
            await Task.CompletedTask;
        }

        private static void SaveTokens(HttpContext ctx, LoginCommandResponse data)
        {
            var opts = new CookieOptions { HttpOnly = true, Secure = false, SameSite = SameSiteMode.Lax, Expires = data.Expiration };
            ctx.Response.Cookies.Append("flix_access_token", data.Token, opts);
            ctx.Response.Cookies.Append("flix_refresh_token", data.RefreshToken, opts);
        }

        private static ClaimsPrincipal BuildPrincipalFromJwt(string jwt)
        {
            var token = new JwtSecurityTokenHandler().ReadJwtToken(jwt);
            var id = new ClaimsIdentity("Cookies");
            foreach (var c in token.Claims) id.AddClaim(c);
            return new ClaimsPrincipal(id);
        }
    }
}
