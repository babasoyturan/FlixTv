using FlixTv.Clients.WebApp.Services.Http;
using FlixTv.Common.Models.RequestModels.Users;

namespace FlixTv.Clients.WebApp.Services.Abstractions
{
    public interface IUsersService
    {
        Task<ApiResult<string>> UpdateAsync(UpdateUserCommandRequest request, CancellationToken ct = default);

        Task<ApiResult<string>> ChangePasswordAsync(ChangePasswordCommandRequest request, CancellationToken ct = default);
    }
}
