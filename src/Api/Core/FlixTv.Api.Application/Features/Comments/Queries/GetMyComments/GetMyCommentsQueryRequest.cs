using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.ResponseModels.Comments;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Comments.Queries.GetMyComments
{
    public class GetMyCommentsQueryRequest : IRequest<IList<GetCommentQueryResponse>>
    {
        public Func<IQueryable<Comment>, IOrderedQueryable<Comment>>? orderBy { get; set; } = null;

        public int currentPage { get; set; } = 0;

        public int pageSize { get; set; } = 0;
    }
}
