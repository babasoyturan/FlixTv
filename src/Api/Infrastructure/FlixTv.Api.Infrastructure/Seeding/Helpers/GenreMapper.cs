using FlixTv.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Infrastructure.Seeding.Helpers
{
    public static class GenreMapper
    {
        private static readonly Dictionary<int, MovieCategory> Map = new()
    {
        { 28,  MovieCategory.Action },
        { 16,  MovieCategory.Animation },
        { 35,  MovieCategory.Comedy },
        { 80,  MovieCategory.Crime },
        { 18,  MovieCategory.Drama },
        { 14,  MovieCategory.Fantasy },
        { 36,  MovieCategory.Historical },
        { 27,  MovieCategory.Horror },
        { 10749, MovieCategory.Romance },
        { 878,  MovieCategory.ScienceFiction },
        { 53,   MovieCategory.Thriller },
        { 37,   MovieCategory.Western }
    };

        public static List<MovieCategory> MapToEnumCategories(IEnumerable<int> tmdbGenreIds)
            => tmdbGenreIds.Select(id => Map.TryGetValue(id, out var mc) ? mc : (MovieCategory?)null)
                           .Where(x => x.HasValue)
                           .Select(x => x!.Value)
                           .Distinct()
                           .ToList();
    }
}
