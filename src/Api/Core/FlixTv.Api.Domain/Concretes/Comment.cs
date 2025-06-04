using FlixTv.Api.Domain.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Domain.Concretes
{
    public class Comment : EntityBase
    {
        public int AuthorId { get; set; }
        public int MovieId { get; set; }
        public string Message { get; set; }
        public int LikeCount { get; set; } = 0;
        public int DislikeCount { get; set; } = 0;
        public Movie? Movie { get; set; } = null;
        public User? Author { get; set; } = null;

        public Comment()
        {
            
        }

        public Comment(
            int authorId, 
            int movieId, 
            string message)
        {
            AuthorId = authorId;
            MovieId = movieId;
            Message = message;
        }
    }
}
