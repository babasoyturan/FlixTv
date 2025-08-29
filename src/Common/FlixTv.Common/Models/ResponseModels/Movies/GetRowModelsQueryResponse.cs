using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Common.Models.ResponseModels.Movies
{
    public class GetRowModelsQueryResponse
    {
        public RowType Type { get; init; }
        public string Title { get; init; } = default!;
        public string RowKey { get; init; } = default!;

        public bool ExcludeWatched { get; init; } = true;

        public List<int>? Genres { get; init; }
        public int? SeedMovieId { get; init; }
        public int? YearFrom { get; init; }
        public int? YearTo { get; init; }
        public string? Key { get; init; }
        public int? Seed { get; init; }
    }
}
