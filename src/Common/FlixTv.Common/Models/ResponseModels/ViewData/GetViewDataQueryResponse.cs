using FlixTv.Common.Models.DTOs;
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
        public bool IsCompleted { get; set; }
        public int WatchedSeconds { get; set; }
        public AuthorDto User { get; set; }
        public MovieDto Movie { get; set; }
        public DateTime LastWatchedAt { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
