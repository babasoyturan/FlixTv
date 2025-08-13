using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.ResponseModels.Users;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Users.Queries.GetCurrentUser
{
    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQueryRequest, GetUserQueryResponse>
    {
        private readonly int userId;
        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;

        public GetCurrentUserQueryHandler(IHttpContextAccessor httpContextAccessor, IMapper mapper, UserManager<User> userManager)
        {
            this.userId = Convert.ToInt32(httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            this.mapper = mapper;
            this.userManager = userManager;
        }

        public async Task<GetUserQueryResponse> Handle(GetCurrentUserQueryRequest request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            var response = mapper.Map<GetUserQueryResponse, User>(user);

            response.Roles = await userManager.GetRolesAsync(user);

            return response;
        }
    }
}
