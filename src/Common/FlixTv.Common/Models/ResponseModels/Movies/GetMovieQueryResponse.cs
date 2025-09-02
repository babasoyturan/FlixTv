using FlixTv.Common.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FlixTv.Common.Models.ResponseModels.Movies
{
    public class GetMovieQueryResponse
    {
        public int Id { get; set; }
        public string SourceVideoUrl { get; set; }
        public string SubtitleUrl { get; set; } 
        public string TrailerVideoUrl { get; set; }
        public string BannerImageUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ReleaseYear { get; set; }
        public int Duration { get; set; }
        public short AgeLimitation { get; set; }
        public float Rating { get; set; }
        public int LastPositionSeconds { get; set; }
        public ICollection<string> Categories { get; set; }
    }
}
