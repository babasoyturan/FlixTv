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
        public required int AuthorId { get; set; }
        public required int MovieId { get; set; }
        public required string Title { get; set; }
        public required string Message { get; set; }
        public required byte RatingPoint { get; set; }
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
