using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FlixTv.Common.Models.RequestModels.Movies
{
    public class CreateMovieCommandRequest : IRequest<Unit>
    {
        public int? TmdbId { get; set; }
        public string CoverImageUrl { get; set; }
        public string BannerImageUrl { get; set; }
        public string SourceVideoUrl { get; set; }
        public string TrailerVideoUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ReleaseYear { get; set; }
        public int Duration { get; set; }
        public short AgeLimitation { get; set; }
        public bool IsVisible { get; set; }
        public float? InitialRating { get; set; }
        public ICollection<MovieCategory> Categories { get; set; }
    }
}
