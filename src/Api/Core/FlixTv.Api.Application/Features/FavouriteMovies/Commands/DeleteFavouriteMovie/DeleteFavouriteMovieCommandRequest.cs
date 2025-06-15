using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.FavouriteMovies.Commands.DeleteFavouriteMovie
{
    public class DeleteFavouriteMovieCommandRequest : IRequest<Unit>
    {
        public int FavouriteMovieId { get; set; }
    }
}
