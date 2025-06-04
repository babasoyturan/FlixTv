using FlixTv.Api.Domain.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Domain.Concretes
{
    public class UserMovieCatalog : EntityBase
    {
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public Movie? Movie { get; set; }
        public User? User { get; set; }

        public UserMovieCatalog()
        {
            
        }

        public UserMovieCatalog(
            int userId, 
            int movieId)
        {
            UserId = userId;
            MovieId = movieId;
        }
    }
}
