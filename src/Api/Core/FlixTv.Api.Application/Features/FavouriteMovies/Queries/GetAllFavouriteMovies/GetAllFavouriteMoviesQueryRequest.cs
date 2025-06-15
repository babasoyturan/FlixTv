using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.ResponseModels.FavouriteMovies;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.FavouriteMovies.Queries.GetAllFavouriteMovies
{
    public class GetAllFavouriteMoviesQueryRequest : IRequest<IList<GetFavouriteMovieQueryResponse>>
    {
        public Expression<Func<UserMovieCatalog, bool>> predicate { get; set; } = p => true;

        public Func<IQueryable<UserMovieCatalog>, IOrderedQueryable<UserMovieCatalog>>? orderBy { get; set; } = null;

        public int currentPage { get; set; } = 0;

        public int pageSize { get; set; } = 0;
    }
}
