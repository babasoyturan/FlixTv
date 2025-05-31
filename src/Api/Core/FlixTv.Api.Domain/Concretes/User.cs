using FlixTv.Api.Domain.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Domain.Concretes
{
    public class User
    {
        public ICollection<UserMovieCatalog>? FavoriteMovies { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<ViewData>? WatchedHistory { get; set; }
    }
}
