using FlixTv.Api.Domain.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Domain.Concretes
{
    public class ViewData : EntityBase
    {
        public required int UserId { get; set; }
        public required int MovieId { get; set; }
        public Movie? Movie { get; set; }
        public User? User { get; set; }

        public ViewData()
        {
            
        }

        public ViewData(
            int userId, 
            int movieId)
        {
            UserId = userId;
            MovieId = movieId;
        }
    }
}
