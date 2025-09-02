using FlixTv.Common.Models.RequestModels.ViewDatas;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.ViewDatas.Commands.CreateViewData
{
    public class CreateViewDataCommandValidator : AbstractValidator<CreateViewDataCommandRequest>
    {
        public CreateViewDataCommandValidator()
        {
            RuleFor(v => v.MovieId)
                .NotEmpty().WithMessage("MovieId must not be empty.")
                .GreaterThan(0).WithMessage("MovieId must be greater than 0.");

            RuleFor(v => v.LastPositionSeconds)
                .GreaterThanOrEqualTo(0).WithMessage("LastPositionSeconds cannot be negative.");

            RuleFor(v => v.MaxPositionSeconds)
                .GreaterThanOrEqualTo(0).WithMessage("MaxPositionSeconds cannot be negative.");

            RuleFor(v => v.WatchedSeconds)
                .GreaterThanOrEqualTo(0).WithMessage("WatchedSeconds cannot be negative.");

            RuleFor(v => v)
                .Must(v => v.MaxPositionSeconds >= v.LastPositionSeconds)
                .WithMessage("MaxPositionSeconds cannot be less than LastPositionSeconds.");
        }
    }
}
