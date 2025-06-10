using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Common.Models.RequestModels.Comments
{
    public class UpdateCommentCommandRequest : IRequest
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public int LikeCount { get; set; }
        public int DislikeCount { get; set; }
    }
}
