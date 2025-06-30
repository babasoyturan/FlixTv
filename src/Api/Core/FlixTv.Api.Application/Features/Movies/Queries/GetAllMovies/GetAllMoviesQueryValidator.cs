using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Movies.Queries.GetAllMovies
{
    public class GetAllMoviesQueryValidator : AbstractValidator<GetAllMoviesQueryRequest>
    {
        public GetAllMoviesQueryValidator()
        {
            RuleFor(r => r.userId)
                .GreaterThanOrEqualTo(0)
                .WithMessage("User ID must be greater than or equal to 0.");
        }
    }
}
