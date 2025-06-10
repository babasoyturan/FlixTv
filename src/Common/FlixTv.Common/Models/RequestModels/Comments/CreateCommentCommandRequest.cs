using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Common.Models.RequestModels.Comments
{
    public class CreateCommentCommandRequest : IRequest<Unit>
    {
        public int AuthorId { get; set; }
        public int MovieId { get; set; }
        public string Message { get; set; }
    }
}
