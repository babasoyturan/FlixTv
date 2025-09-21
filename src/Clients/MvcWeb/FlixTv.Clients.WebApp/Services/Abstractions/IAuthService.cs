using FlixTv.Clients.WebApp.Services.Http;
using FlixTv.Common.Models.RequestModels.Auth;
using FlixTv.Common.Models.ResponseModels.Auth;

namespace FlixTv.Clients.WebApp.Services.Abstractions
{
    public interface IAuthService
    {
        Task<ApiResult<LoginCommandResponse>> LoginAsync(LoginCommandRequest request, HttpContext ctx);

        Task<ApiResult<string>> RegisterAsync(RegisterCommandRequest request);

        Task<ApiResult<string>> ForgotPasswordAsync(ForgotPasswordCommandRequest request);

        Task<ApiResult<string>> ResetPasswordAsync(ResetPasswordCommandRequest request);

        Task<ApiResult<string>> ConfirmEmailAsync(string url);

        Task<ApiResult<bool>> RevokeMeAsync();

        Task UpdateCookieClaimsAsync(HttpContext ctx, string? name = null, string? surname = null, string? email = null);

        Task LogoutAsync(HttpContext ctx);
    }
}
