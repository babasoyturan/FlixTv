using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Common.Models.RequestModels.ViewDatas
{
    public class CreateViewDataCommandRequest : IRequest<Unit>
    {
        public int UserId { get; set; }
        public int MovieId { get; set; }
    }
}
