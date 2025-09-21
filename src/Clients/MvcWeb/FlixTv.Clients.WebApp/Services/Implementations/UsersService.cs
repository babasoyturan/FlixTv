using FlixTv.Clients.WebApp.Services.Abstractions;
using FlixTv.Clients.WebApp.Services.Http;
using FlixTv.Common.Models.RequestModels.Users;

namespace FlixTv.Clients.WebApp.Services.Implementations
{
    public class UsersService : BaseApiClient, IUsersService
    {
        private const string UpdateEndpoint = "Users/UpdateUser";
        private const string PwdEndpoint = "Users/ChangePassword";

        public UsersService(IHttpClientFactory factory) : base(factory.CreateClient("flix-api")) { }

        public Task<ApiResult<string>> UpdateAsync(UpdateUserCommandRequest request, CancellationToken ct = default)
            => PostJsonAsync<UpdateUserCommandRequest, string>(UpdateEndpoint, request, ct);

        public Task<ApiResult<string>> ChangePasswordAsync(ChangePasswordCommandRequest request, CancellationToken ct = default)
            => PostJsonAsync<ChangePasswordCommandRequest, string>(PwdEndpoint, request, ct);
    }
}
