using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Infrastructure.Seeding.Options
{
    public sealed class MoviesSeedingOptions
    {
        public bool Enabled { get; set; } = false;
        public int TargetCount { get; set; } = 300;
    }
}
