using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.ResponseModels.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Users.Queries.GetUser
{
    public class GetUserQueryHandler : IRequestHandler<GetUserQueryRequest, GetUserQueryResponse>
    {
        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;

        public GetUserQueryHandler(IMapper mapper, UserManager<User> userManager)
        {
            this.mapper = mapper;
            this.userManager = userManager;
        }

        public async Task<GetUserQueryResponse> Handle(GetUserQueryRequest request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.UserId.ToString());

            if (user is null)
                throw new KeyNotFoundException($"User with ID {request.UserId} not found.");

            var response = mapper.Map<GetUserQueryResponse, User>(user);

            response.Roles = await userManager.GetRolesAsync(user);

            return response;
        }
    }
}
