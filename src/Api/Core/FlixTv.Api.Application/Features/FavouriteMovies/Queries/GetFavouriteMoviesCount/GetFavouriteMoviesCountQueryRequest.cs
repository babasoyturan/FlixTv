using FlixTv.Api.Domain.Concretes;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.FavouriteMovies.Queries.GetFavouriteMoviesCount
{
    public class GetFavouriteMoviesCountQueryRequest : IRequest<int>
    {
        public Expression<Func<UserMovieCatalog, bool>> predicate { get; set; } = p => true;
    }
}