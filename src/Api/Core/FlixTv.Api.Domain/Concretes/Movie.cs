using FlixTv.Api.Domain.Abstracts;
using FlixTv.Common.ViewModels;
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
        public required string SourceVideoUrl { get; set; }
        public required string TrailerVideoUrl { get; set; }
        public required string CoverImageUrl { get; set; }
        public required string BannerImageUrl { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required int ReleaseYear { get; set; }
        public required int Duration { get; set; }
        public required short AgeLimitation { get; set; }
        public required ICollection<MovieCategory> Categories { get; set; }
        public bool IsVisible { get; set; } = true;
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
