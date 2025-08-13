using FlixTv.Common.Models.RequestModels.Users;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Users.Commands.ChangeUserStatus
{
    public class ChangeUserStatusCommandValidator : AbstractValidator<ChangeUserStatusCommandRequest>
    {
        public ChangeUserStatusCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID cannot be empty.")
                .GreaterThan(0).WithMessage("User ID must be greater than zero.");
        }
    }
}
