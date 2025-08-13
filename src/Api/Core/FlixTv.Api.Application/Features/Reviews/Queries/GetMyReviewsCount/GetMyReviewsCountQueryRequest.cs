using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Reviews.Queries.GetMyReviewsCount
{
    public class GetMyReviewsCountQueryRequest : IRequest<int>
    {
    }
}
