using FlixTv.Common.Models.ResponseModels.FavouriteMovies;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.FavouriteMovies.Queries.GetFavouriteMovie
{
    public class GetFavouriteMovieQueryRequest : IRequest<GetFavouriteMovieQueryResponse>
    {
        public int FavouriteMovieId { get; set; }
    }
}
