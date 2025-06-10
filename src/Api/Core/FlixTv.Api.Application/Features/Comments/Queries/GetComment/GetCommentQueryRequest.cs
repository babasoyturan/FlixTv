using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.ResponseModels.Comments;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Comments.Queries.GetComment
{
    public class GetCommentQueryRequest : IRequest<GetCommentQueryResponse>
    {
        public int CommentId { get; set; }
    }
}