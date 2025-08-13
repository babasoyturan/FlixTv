using FlixTv.Common.Models.RequestModels.Users;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Users.Commands.ResetUserRole
{
    public class ResetUserRoleCommandValidator : AbstractValidator<ResetUserRoleCommandRequest>
    {
        public ResetUserRoleCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User ID cannot be empty.")
                .GreaterThan(0)
                .WithMessage("User ID must be greater than zero.");

            RuleFor(x => x.Role)
                .NotEmpty()
                .WithMessage("Role cannot be empty.")
                .Must(role => role.Equals("Moderator", StringComparison.OrdinalIgnoreCase) || 
                              role.Equals("User", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Role must be either 'Moderator', or 'User'.");
        }
    }
}
