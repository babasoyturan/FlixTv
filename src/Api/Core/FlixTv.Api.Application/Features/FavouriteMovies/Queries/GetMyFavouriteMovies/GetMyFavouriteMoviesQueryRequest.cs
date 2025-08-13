using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.ResponseModels.FavouriteMovies;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.FavouriteMovies.Queries.GetMyFavouriteMovies
{
    public class GetMyFavouriteMoviesQueryRequest : IRequest<IList<GetFavouriteMovieQueryResponse>>
    {
        public Func<IQueryable<UserMovieCatalog>, IOrderedQueryable<UserMovieCatalog>>? orderBy { get; set; } = null;

        public int currentPage { get; set; } = 0;

        public int pageSize { get; set; } = 0;
    }
}
