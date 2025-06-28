using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Common.Models.DTOs
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public int LikeCount { get; set; } = 0;
        public int DislikeCount { get; set; } = 0;
        public bool IsLiked { get; set; } = false;
        public bool IsDisliked { get; set; } = false;
        public DateTime CreatedDate { get; set; }
        public AuthorDto Author { get; set; }
    }
}
