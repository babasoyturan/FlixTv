using FlixTv.Common.Models.ResponseModels.Reviews;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Reviews.Queries.GetReview
{
    public class GetReviewQueryRequest : IRequest<GetReviewQueryResponse>
    {
        public int ReviewId { get; set; }
    }
}
