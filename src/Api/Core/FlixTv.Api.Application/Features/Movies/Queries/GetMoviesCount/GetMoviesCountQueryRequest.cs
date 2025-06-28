using FlixTv.Api.Domain.Concretes;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Movies.Queries.GetMoviesCount
{
    public class GetMoviesCountQueryRequest : IRequest<int>
    {
        public Expression<Func<Movie, bool>> predicate { get; set; } = p => true;
    }
}
