using FlixTv.Api.Application.Interfaces.Tokens;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.RequestModels.Auth;
using FlixTv.Common.Models.ResponseModels.Auth;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommandRequest, LoginCommandResponse>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ITokenService tokenService;
        private readonly IConfiguration configuration;
        private readonly UserManager<User> userManager;

        public LoginCommandHandler(
            IUnitOfWork unitOfWork, 
            IHttpContextAccessor httpContextAccessor, 
            ITokenService tokenService, 
            IConfiguration configuration, 
            UserManager<User> userManager
        )
        {
            this.unitOfWork = unitOfWork;
            this.httpContextAccessor = httpContextAccessor;
            this.tokenService = tokenService;
            this.configuration = configuration;
            this.userManager = userManager;
        }

        public async Task<LoginCommandResponse> Handle(LoginCommandRequest request, CancellationToken cancellationToken)
        {
            var errorMessage = "Email or password is wrong";
            var user = await userManager.FindByEmailAsync(request.Email);

            if (user is null)
                throw new Exception(errorMessage);

            var checkPassword = await userManager.CheckPasswordAsync(user, request.Password);

            if (!checkPassword)
                throw new Exception(errorMessage);

            var roles = await userManager.GetRolesAsync(user);

            var token = await tokenService.CreateToken(user, roles);

            var refreshToken = tokenService.GenerateRefreshToken();

            _ = int.TryParse(configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(refreshTokenValidityInDays);

            await userManager.UpdateAsync(user);
            await userManager.UpdateSecurityStampAsync(user);

            var _token = new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginCommandResponse
            {
                Token = _token,
                RefreshToken = refreshToken,
                Expiration = token.ValidTo
            };
        }
    }
}
