using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Infrastructure.Seeding.Options
{
    public sealed class TmdbOptions
    {
        public string ApiKey { get; set; } = default!;
        public string ApiReadAccessToken { get; set; } = default!;
        public string ImageBase { get; set; } = "https://image.tmdb.org/t/p/";
        public string PosterSize { get; set; } = "w500";
        public string BackdropSize { get; set; } = "w1280";
    }
}
