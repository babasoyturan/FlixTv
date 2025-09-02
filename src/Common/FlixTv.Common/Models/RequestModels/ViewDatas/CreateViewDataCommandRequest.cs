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
        public int MovieId { get; set; }

        public int LastPositionSeconds { get; set; }

        public int MaxPositionSeconds { get; set; }

        public int WatchedSeconds { get; set; }
    }
}
