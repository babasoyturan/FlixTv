using FlixTv.Api.Domain.Concretes;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Comments.Queries.GetCommentsCount
{
    public class GetCommentsCountQueryRequest : IRequest<int>
    {
        public Expression<Func<Comment, bool>> predicate { get; set; } = p => true;
    }
}
