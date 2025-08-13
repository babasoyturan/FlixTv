using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Comments.Queries.GetMyCommentsCount
{
    public class GetMyCommentsCountQueryRequest : IRequest<int>
    {
    }
}
