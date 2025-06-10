using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.ResponseModels.Comments;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Comments.Queries.GetAllComments
{
    public class GetAllCommentsQueryRequest : IRequest<IList<GetCommentQueryResponse>>
    {
        public Expression<Func<Comment, bool>> predicate { get; set; } = p => true;

        public Func<IQueryable<Comment>, IOrderedQueryable<Comment>>? orderBy { get; set; } = null;

        public int currentPage { get; set; } = 0;

        public int pageSize { get; set; } = 0;
    }
}
