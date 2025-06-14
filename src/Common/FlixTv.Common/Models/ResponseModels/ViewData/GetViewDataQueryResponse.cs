using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Common.Models.ResponseModels.ViewData
{
    public class GetViewDataQueryResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public DateTime CreatedDat { get; set; }
    }
}
