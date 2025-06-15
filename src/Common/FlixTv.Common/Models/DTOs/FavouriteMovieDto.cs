using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Common.Models.DTOs
{
    public class FavouriteMovieDto
    {
        public int Id { get; set; }
        public string CoverImageUrl { get; set; }
        public string Title { get; set; }
        public int ReleaseYear { get; set; }
        public float Rating { get; set; }
        public ICollection<MovieCategory> Categories { get; set; }
    }
}
