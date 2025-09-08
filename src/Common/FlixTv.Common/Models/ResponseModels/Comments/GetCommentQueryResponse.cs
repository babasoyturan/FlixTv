using FlixTv.Common.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Common.Models.ResponseModels.Comments
{
    public class GetCommentQueryResponse
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public int LikeCount { get; set; } = 0;
        public int DislikeCount { get; set; } = 0;
        public bool HasLiked { get; set; } = false;
        public bool HasDisliked { get; set; } = false;
        public DateTime CreatedDate { get; set; }
        public AuthorDto Author { get; set; }
        public MovieDto Movie { get; set; }
    }
}