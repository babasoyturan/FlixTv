using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Movies.Queries.GetMoviesByUserCompatibility
{
    public class GetMoviesByUserCompatibilityQueryValidator : AbstractValidator<GetMoviesByUserCompatibilityQueryRequest>
    {
        public GetMoviesByUserCompatibilityQueryValidator()
        {
            RuleFor(m => m.userId)
                .NotEmpty()
                .GreaterThan(0);
        }
    }
}
