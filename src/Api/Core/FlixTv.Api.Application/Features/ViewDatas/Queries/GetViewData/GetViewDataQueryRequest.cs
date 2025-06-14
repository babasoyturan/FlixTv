using FlixTv.Common.Models.ResponseModels.ViewData;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.ViewDatas.Queries.GetViewData
{
    public class GetViewDataQueryRequest : IRequest<GetViewDataQueryResponse>
    {
        public int ViewDataId { get; set; }
    }
}
