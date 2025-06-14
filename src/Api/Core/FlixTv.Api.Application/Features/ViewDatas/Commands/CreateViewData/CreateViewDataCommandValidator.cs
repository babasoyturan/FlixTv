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
            RuleFor(v => v.UserId)
                .NotEmpty()
                .GreaterThan(0);

            RuleFor(v => v.MovieId)
                .NotEmpty()
                .GreaterThan(0);
        }
    }
}
