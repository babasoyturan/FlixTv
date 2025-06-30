using FlixTv.Common.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Common.Models.ResponseModels.Movies
{
    public class GetAllMoviesQueryResponse
    {
        public int Id { get; set; }
        public bool IsFavourite { get; set; }
        public string CoverImageUrl { get; set; }
        public string Title { get; set; }
        public int ReleaseYear { get; set; }
        public float Rating { get; set; }
        public bool IsVisible { get; set; }
        public int ViewCount { get; set; }
        public ICollection<string> Categories { get; set; }
    }
}
