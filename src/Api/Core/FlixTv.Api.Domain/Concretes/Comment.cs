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
        public IList<int> Likes { get; set; } = new List<int>();
        public IList<int> Dislikes { get; set; } = new List<int>();
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
