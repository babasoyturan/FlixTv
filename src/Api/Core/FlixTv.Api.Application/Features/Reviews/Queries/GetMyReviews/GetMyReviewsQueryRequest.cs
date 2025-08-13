using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.ResponseModels.Reviews;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Reviews.Queries.GetMyReviews
{
    public class GetMyReviewsQueryRequest : IRequest<IList<GetReviewQueryResponse>>
    {
        public Func<IQueryable<Review>, IOrderedQueryable<Review>>? orderBy { get; set; } = null;

        public int currentPage { get; set; } = 0;

        public int pageSize { get; set; } = 0;
    }
}
