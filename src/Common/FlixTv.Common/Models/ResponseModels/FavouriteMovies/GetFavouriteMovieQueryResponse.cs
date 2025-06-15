using FlixTv.Common.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Common.Models.ResponseModels.FavouriteMovies
{
    public class GetFavouriteMovieQueryResponse
    {
        public FavouriteMovieDto Movie { get; set; }
    }
}
