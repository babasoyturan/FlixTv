using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Reviews.Commands.DeleteReview
{
    public class DeleteReviewCommandRequest : IRequest<Unit>
    {
        public int ReviewId { get; set; }
    }
}
