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

        public Task<ApiResult<bool>> RevokeMeAsync() => PostAsync("Auth/RevokeMe");

        // Profil info dəyişəndə UI cookie principal-ı yenilə
        public async Task UpdateCookieClaimsAsync(HttpContext ctx, string? name = null, string? surname = null, string? email = null)
        {
            var old = ctx.User?.Claims ?? Enumerable.Empty<Claim>();

            // “name”, “surname”, Email claim-lərini çıxarıb qalanlarını saxla
            var filtered = old.Where(c =>
                !string.Equals(c.Type, "name", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(c.Type, "surname", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(c.Type, ClaimTypes.Email, StringComparison.OrdinalIgnoreCase));

            var id = new ClaimsIdentity("Cookies");
            id.AddClaims(filtered);

            // Yenisini əlavə et (göndərilməyənləri köhnədən saxla)
            var finalName = name ?? old.FirstOrDefault(c => c.Type == "name")?.Value;
            var finalSurname = surname ?? old.FirstOrDefault(c => c.Type == "surname")?.Value;
            var finalEmail = email ?? old.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (!string.IsNullOrWhiteSpace(finalName)) id.AddClaim(new Claim("name", finalName));
            if (!string.IsNullOrWhiteSpace(finalSurname)) id.AddClaim(new Claim("surname", finalSurname));
            if (!string.IsNullOrWhiteSpace(finalEmail)) id.AddClaim(new Claim(ClaimTypes.Email, finalEmail));

            var principal = new ClaimsPrincipal(id);
            await ctx.SignInAsync("Cookies", principal);
        }

        public async Task LogoutAsync(HttpContext ctx)
        {
            try { await RevokeMeAsync(); } catch { /* ignore */ }
            ctx.Response.Cookies.Delete("flix_access_token");
            ctx.Response.Cookies.Delete("flix_refresh_token");
            await ctx.SignOutAsync("Cookies");
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
