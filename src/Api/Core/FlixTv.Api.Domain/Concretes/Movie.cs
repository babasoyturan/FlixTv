using FlixTv.Api.Domain.Abstracts;
using FlixTv.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FlixTv.Api.Domain.Concretes
{
    public class Movie : EntityBase
    {
        public string SourceVideoUrl { get; set; }
        public string TrailerVideoUrl { get; set; }
        public string CoverImageUrl { get; set; }
        public string BannerImageUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ReleaseYear { get; set; }
        public int Duration { get; set; }
        public short AgeLimitation { get; set; }
        public bool IsVisible { get; set; } = true;
        public ICollection<MovieCategory> Categories { get; set; }
        public ICollection<ViewData>? Views { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<UserMovieCatalog>? FavoriteMovieUsers { get; set; }

        public Movie()
        {
            
        }

        public Movie(
            string sourceVideoUrl, 
            string trailerVideoUrl, 
            string coverImageUrl, 
            string bannerImageUrl, 
            string title, 
            string description, 
            int releaseYear, 
            int duration, 
            short ageLimitation, 
            ICollection<MovieCategory> categories, 
            bool isVisible = true)
        {
            SourceVideoUrl = sourceVideoUrl;
            TrailerVideoUrl = trailerVideoUrl;
            CoverImageUrl = coverImageUrl;
            BannerImageUrl = bannerImageUrl;
            Title = title;
            Description = description;
            ReleaseYear = releaseYear;
            Duration = duration;
            AgeLimitation = ageLimitation;
            Categories = categories;
            IsVisible = isVisible;
        }
    }
}
