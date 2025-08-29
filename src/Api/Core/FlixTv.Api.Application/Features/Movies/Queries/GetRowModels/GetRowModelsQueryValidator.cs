using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Movies.Queries.GetRowModels
{
    public class GetRowModelsQueryValidator : AbstractValidator<GetRowModelsQueryRequest>
    {
        public GetRowModelsQueryValidator()
        {
            RuleFor(x => x.Count)
                .GreaterThan(0).WithMessage("Count must be greater than 0.")
                .LessThanOrEqualTo(50).WithMessage("Count must be 50 or less.");
        }
    }
}
