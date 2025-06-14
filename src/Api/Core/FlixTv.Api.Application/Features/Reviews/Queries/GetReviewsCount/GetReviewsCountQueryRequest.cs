using FlixTv.Api.Domain.Concretes;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Reviews.Queries.GetReviewsCount
{
    public class GetReviewsCountQueryRequest : IRequest<int>
    {
        public Expression<Func<Review, bool>> predicate { get; set; } = p => true;
    }
}
