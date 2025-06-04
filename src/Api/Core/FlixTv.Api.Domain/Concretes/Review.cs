using FlixTv.Api.Domain.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Domain.Concretes
{
    public class Review : EntityBase
    {
        public int AuthorId { get; set; }
        public int MovieId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public byte RatingPoint { get; set; }
        public Movie? Movie { get; set; }
        public User? Author { get; set; }

        public Review()
        {
            
        }

        public Review(
            int authorId, 
            int movieId, 
            string title, 
            string message, 
            byte ratingPoint)
        {
            AuthorId = authorId;
            MovieId = movieId;
            Title = title;
            Message = message;
            RatingPoint = ratingPoint;
        }
    }
}
