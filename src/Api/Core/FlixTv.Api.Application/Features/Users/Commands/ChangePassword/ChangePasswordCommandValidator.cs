using FlixTv.Common.Models.RequestModels.Users;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Users.Commands.ChangePassword
{
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommandRequest>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(x => x.OldPassword)
                .NotEmpty()
                .WithMessage("Old password is required.")
                .MinimumLength(2)
                .WithMessage("Old password must be at least 2 characters long.")
                .MaximumLength(100)
                .WithMessage("Old password must not exceed 100 characters.");

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage("New password is required.")
                .MinimumLength(2)
                .WithMessage("New password must be at least 2 characters long.")
                .MaximumLength(100)
                .WithMessage("New password must not exceed 100 characters.")
                .NotEqual(x => x.OldPassword)
                .WithMessage("New password must be different from old password.");
        }
    }
}
