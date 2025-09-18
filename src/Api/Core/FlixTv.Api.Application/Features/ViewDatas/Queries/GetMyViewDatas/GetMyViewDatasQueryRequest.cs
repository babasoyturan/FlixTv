using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.ResponseModels.ViewData;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.ViewDatas.Queries.GetMyViewDatas
{
    public class GetMyViewDatasQueryRequest : IRequest<IList<GetViewDataQueryResponse>>
    {
        public Func<IQueryable<ViewData>, IOrderedQueryable<ViewData>>? orderBy { get; set; } = null;

        public int currentPage { get; set; } = 0;

        public int pageSize { get; set; } = 0;
    }
}
