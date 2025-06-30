using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Movies.Queries.GetRelatedMovies
{
    public class GetRelatedMoviesQueryValidator : AbstractValidator<GetRelatedMoviesQueryRequest>
    {
        public GetRelatedMoviesQueryValidator()
        {
            RuleFor(m => m.UserId)
                .NotNull()
                .GreaterThanOrEqualTo(0);

            RuleFor(m => m.MovieId)
                .NotNull()
                .GreaterThan(0);

            RuleFor(m => m.Size)
                .NotEmpty()
                .GreaterThan(0);
        }
    }
}
