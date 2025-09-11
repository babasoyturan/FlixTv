using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Common.Models
{
    public enum MovieCategory
    {
        Action,
        Animation,
        Comedy,
        Crime,
        Drama,
        Fantasy,
        Historical,
        Horror,
        Romance,
        ScienceFiction,
        Thriller,
        Western
    }

    public enum RowType
    {
        SpecialKey,
        TopGenres,
        ComboGenres,
        SimilarToMovie,
        Decade,
        ContinueWatching
    }

    public enum SourceType
    {
        FlixTv = 0,
        Tmdb = 1
    }
}
