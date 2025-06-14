using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.ViewDatas.Commands.DeleteViewData
{
    public class DeleteViewDataCommandRequest : IRequest<Unit>
    {
        public int ViewDataId { get; set; }
    }
}
