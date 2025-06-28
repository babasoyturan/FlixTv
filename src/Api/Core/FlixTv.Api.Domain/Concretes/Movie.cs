using FlixTv.Api.Domain.Abstracts;
using FlixTv.Common.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
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
        public float Rating { get; set; }
        public double[] FeatureVector { get; set; }
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

        public void SetFeatureVector()
        {
            List<MovieCategory> _allCategories = Enum.GetValues(typeof(MovieCategory)).Cast<MovieCategory>().ToList();
            var _minReleaseYear = 1900;
            var _maxReleaseYear = 3000;
            var _yearRange = (_maxReleaseYear - _minReleaseYear) > 0 ? (_maxReleaseYear - _minReleaseYear) : 1;

            var vec = new double[_allCategories.Count + 1];

            foreach (var cat in Categories)
            {
                int idx = _allCategories.IndexOf(cat);
                if (idx >= 0)
                    vec[idx] = 1;
            }

            double normalizedYear = (ReleaseYear - _minReleaseYear) / _yearRange;
            vec[^1] = Math.Clamp(normalizedYear, 0.0, 1.0);

            FeatureVector = vec;
        }

        public void SetMovieRating() => Rating = Reviews is not null && Reviews.Count() > 0 ? (float)Math.Round((double)Reviews.Sum(r => r.RatingPoint) / Reviews.Count(), 1) : 0;
    }
}
