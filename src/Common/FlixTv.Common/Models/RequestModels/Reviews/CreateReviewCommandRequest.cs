using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Common.Models.RequestModels.Reviews
{
    public class CreateReviewCommandRequest : IRequest<Unit>
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public byte RatingPoint { get; set; }
    }
}
