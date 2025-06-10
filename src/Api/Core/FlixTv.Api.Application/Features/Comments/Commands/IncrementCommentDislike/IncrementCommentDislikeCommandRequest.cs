using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Comments.Commands.IncrementCommentDislike
{
    public class IncrementCommentDislikeCommandRequest : IRequest
    {
        public int CommentId { get; set; }
    }
}
