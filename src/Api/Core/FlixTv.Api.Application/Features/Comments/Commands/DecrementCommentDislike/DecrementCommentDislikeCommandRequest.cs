using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Comments.Commands.DecrementCommentDislike
{
    public class DecrementCommentDislikeCommandRequest : IRequest<Unit>
    {
        public int CommentId { get; set; }
        public int UserId { get; set; }
    }
}
