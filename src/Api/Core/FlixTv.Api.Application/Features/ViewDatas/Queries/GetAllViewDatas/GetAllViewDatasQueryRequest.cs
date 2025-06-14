using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.ResponseModels.ViewData;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.ViewDatas.Queries.GetAllViewDatas
{
    public class GetAllViewDatasQueryRequest : IRequest<IList<GetViewDataQueryResponse>>
    {
        public Expression<Func<ViewData, bool>> predicate { get; set; } = p => true;

        public Func<IQueryable<ViewData>, IOrderedQueryable<ViewData>>? orderBy { get; set; } = null;

        public int currentPage { get; set; } = 0;

        public int pageSize { get; set; } = 0;
    }
}
