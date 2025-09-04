using FlixTv.Api.Infrastructure.Seeding.Tmdb;
using FlixTv.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Infrastructure.Seeding.Helpers
{
    public static class AgeRatingHelper
    {
        public static short FromCertOrHeuristic(TmdbClient.TmdbReleaseDates? rel, ICollection<MovieCategory> cats)
        {
            var cert = rel?.Results?
                .FirstOrDefault(r => r.Iso_3166_1 is "US" or "GB")
                ?.Release_Dates?.Select(rd => rd.Certification)
                .FirstOrDefault(c => !string.IsNullOrWhiteSpace(c))
                ?.ToUpperInvariant();

            if (!string.IsNullOrWhiteSpace(cert))
            {
                if (cert is "G" or "U") return 0;
                if (cert is "PG") return 12;
                if (cert is "PG-13" or "12" or "12A" or "TV-14") return 16;
                if (cert is "R" or "15" or "NC-17" or "18" or "TV-MA") return 18;
            }

            if (cats.Contains(MovieCategory.Horror) || cats.Contains(MovieCategory.Thriller) || cats.Contains(MovieCategory.Crime))
                return 18;
            if (cats.Contains(MovieCategory.Action) || cats.Contains(MovieCategory.ScienceFiction))
                return 16;
            if (cats.Contains(MovieCategory.Romance) || cats.Contains(MovieCategory.Comedy) ||
                cats.Contains(MovieCategory.Animation) || cats.Contains(MovieCategory.Fantasy))
                return 12;

            return 16;
        }
    }
}
