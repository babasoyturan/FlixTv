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
        public IFormFile SourceVideo { get; set; }
        public IFormFile CoverImage { get; set; }
        public IFormFile BannerImage { get; set; }
        public string TrailerVideoUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ReleaseYear { get; set; }
        public int Duration { get; set; }
        public short AgeLimitation { get; set; }
        public bool IsVisible { get; set; }
        public ICollection<MovieCategory> Categories { get; set; }
    }
}
