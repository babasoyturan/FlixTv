using FlixTv.Api.Domain.Concretes;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.ViewDatas.Queries.GetViewDatasCount
{
    public class GetViewDatasCountQueryRequest : IRequest<int>
    {
        public Expression<Func<ViewData, bool>> predicate { get; set; } = p => true;
    }
}
