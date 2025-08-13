using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.RequestModels.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Users.Commands.ResetUserRole
{
    public class ResetUserRoleCommandHandler : IRequestHandler<ResetUserRoleCommandRequest, Unit>
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;

        public ResetUserRoleCommandHandler(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task<Unit> Handle(ResetUserRoleCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
                throw new Exception($"User with ID {request.UserId} not found.");

            var roles = await userManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
                throw new UnauthorizedAccessException("You cannot reset the role of an admin user.");

            if (!await roleManager.RoleExistsAsync(request.Role))
                await roleManager.CreateAsync(new Role
                {
                    Name = request.Role,
                    NormalizedName = request.Role.ToUpperInvariant(),
                });

            if (roles.Count > 0)
            {
                var removeRes = await userManager.RemoveFromRolesAsync(user, roles);
                if (!removeRes.Succeeded)
                    throw new Exception("Failed to remove existing roles.");
            }

            var addRes = await userManager.AddToRoleAsync(user, request.Role);
            if (!addRes.Succeeded)
                throw new Exception("Failed to assign the role.");

            return Unit.Value;
        }
    }
}
