using FlixTv.Common.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Common.Models.ResponseModels.Reviews
{
    public class GetReviewQueryResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public byte RatingPoint { get; set; }
        public DateTime CreatedDate { get; set; }
        public AuthorDto Author { get; set; }
        public MovieDto Movie { get; set; }
    }
}
