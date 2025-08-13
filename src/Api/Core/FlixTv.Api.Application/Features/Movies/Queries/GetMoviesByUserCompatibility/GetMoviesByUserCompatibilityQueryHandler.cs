using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Application.Utilities;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models;
using FlixTv.Common.Models.ResponseModels.Movies;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Movies.Queries.GetMoviesByUserCompatibility
{
    public class GetMoviesByUserCompatibilityQueryHandler : IRequestHandler<GetMoviesByUserCompatibilityQueryRequest, IList<GetAllMoviesQueryResponse>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly int userId;

        private static readonly List<MovieCategory> _allCategories = Enum.GetValues(typeof(MovieCategory)).Cast<MovieCategory>().ToList();
        private const int _minReleaseYear = 1900;
        private const int _maxReleaseYear = 3000;
        private static readonly double _yearRange = (_maxReleaseYear - _minReleaseYear) > 0 ? (_maxReleaseYear - _minReleaseYear) : 1;

        public GetMoviesByUserCompatibilityQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.userId = Convert.ToInt32(httpContextAccessor.HttpContext?.User?.FindFirst("id")?.Value ?? "0");
        }

        public async Task<IList<GetAllMoviesQueryResponse>> Handle(GetMoviesByUserCompatibilityQueryRequest request, CancellationToken cancellationToken)
        {
            var user = await unitOfWork.GetReadRepository<User>().GetAsync(
                u => u.Id == userId, 
                x => x
                .Include(u => u.WatchedHistory).ThenInclude(vd => vd.Movie)
                .Include(u => u.Reviews).ThenInclude(r => r.Movie)
                .Include(u => u.FavoriteMovies).ThenInclude(fm => fm.Movie));

            if (user is null)
                throw new Exception("The user was not found");

            var userVector = BuildUserProfile(user);

            var allMovies = await unitOfWork.GetReadRepository<Movie>().GetAllAsync(
                predicate: m => m.IsVisible,
                include: m => m
                    .Include(m => m.Views)
                    .Include(m => m.Reviews)
                    .Include(m => m.Comments)
            );

            var watchedIds = user.WatchedHistory?.Select(v => v.MovieId).ToHashSet() ?? new();
            var candidateMovies = allMovies.Where(m => !watchedIds.Contains(m.Id)).ToList();

            var twoMonthsAgo = DateTime.UtcNow.AddMonths(-2);

            const double viewWeight = 1.0;
            const double commentLikeWeight = 2.0;
            const double commentDislikeWeight = 1.0;
            const double reviewPosWeight = 8.0;
            const double reviewNegWeight = 12.0;

            var topPopular = candidateMovies
                .Select(m => new
                {
                    Movie = m,
                    Score =
                        m.Views.Count(v => v.CreatedDate >= twoMonthsAgo) * viewWeight
                        + m.Comments
                            .Where(c => c.CreatedDate >= twoMonthsAgo)
                            .Sum(c => c.Likes.Count * commentLikeWeight - c.Dislikes.Count * commentDislikeWeight)
                        + m.Reviews
                            .Where(r => r.CreatedDate >= twoMonthsAgo && r.RatingPoint > 0)
                            .Sum(r => ((double)r.RatingPoint / 10.0) * reviewPosWeight)
                        - m.Reviews
                            .Where(r => r.CreatedDate >= twoMonthsAgo && r.RatingPoint < 6)
                            .Sum(r => ((6.0 - (double)r.RatingPoint) / 6.0) * reviewNegWeight)
                })
                .OrderByDescending(x => x.Score)
                .Take(20)
                .Select(x => x.Movie)
                .ToList();

            var random = candidateMovies
                .Where(m => !topPopular.Contains(m))
                .OrderBy(_ => Guid.NewGuid())
                .Take(20)
                .ToList();

            var longTail = candidateMovies
                .OrderBy(m => m.Views.Count)
                .Take(30)
                .OrderBy(_ => Guid.NewGuid())
                .Take(10)
                .ToList();

            var totalPool = topPopular
                .Concat(random)
                .Concat(longTail)
                .GroupBy(m => m.Id)
                .Select(g => g.First())
                .ToList();

            var recommended = totalPool
                .Select(m => new
                {
                    Movie = m,
                    Score = CosineSimilarity(userVector, m.FeatureVector)
                })
                .OrderByDescending(x => x.Score)
                .Take(request.count)
                .Select(x => x.Movie)
                .ToList();

            var response = mapper.Map<GetAllMoviesQueryResponse, Movie>(recommended);

            for (int i = 0; i < response.Count(); i++)
            {
                response[i].ViewCount = recommended[i].Views.Count();

                var fm = await unitOfWork.GetReadRepository<UserMovieCatalog>().GetAsync(
                    x => x.MovieId == recommended[i].Id && x.UserId == userId);

                response[i].IsFavourite = fm is not null;
            }
                

            return response;
        }

        private double CosineSimilarity(double[] a, double[] b)
        {
            double dot = 0, na = 0, nb = 0;
            for (int i = 0; i < a.Length; i++)
            {
                dot += a[i] * b[i];
                na += a[i] * a[i];
                nb += b[i] * b[i];
            }
            return (na == 0 || nb == 0) ? 0 : dot / (Math.Sqrt(na) * Math.Sqrt(nb));
        }


        private double[] BuildUserProfile(User user)
        {

            var  WeightFavorite = 5.0;
            var WeightViewPerTime = 1.0;

            int dim = _allCategories.Count + 1;
            var profile = new double[dim];
            double totalWeight = 0;

            if (user.FavoriteMovies != null)
            {
                foreach (var fav in user.FavoriteMovies)
                {
                    var m = fav.Movie;
                    if (m == null) continue;

                    var vec = m.FeatureVector;
                    for (int i = 0; i < dim; i++)
                        profile[i] += WeightFavorite * vec[i];

                    totalWeight += WeightFavorite;
                }
            }

            if (user.Reviews != null)
            {
                foreach (var rev in user.Reviews)
                {
                    var m = rev.Movie;
                    if (m == null) continue;

                    double wRev = rev.RatingPoint >= 5
                                  ? rev.RatingPoint
                                  : rev.RatingPoint - 5;

                    var vec = m.FeatureVector;
                    for (int i = 0; i < dim; i++)
                        profile[i] += wRev * vec[i];

                    totalWeight += Math.Abs(wRev);
                }
            }

            if (user.WatchedHistory != null)
            {
                var groups = user.WatchedHistory
                    .Where(v => v.Movie != null)
                    .GroupBy(v => v.Movie!)
                    .ToList();

                foreach (var grp in groups)
                {
                    int count = grp.Count();
                    var m = grp.Key;
                    var vec = m.FeatureVector;
                    double wView = count * WeightViewPerTime;

                    for (int i = 0; i < dim; i++)
                        profile[i] += wView * vec[i];

                    totalWeight += wView;
                }
            }

            if (totalWeight == 0)
                return new double[dim];

            for (int i = 0; i < dim; i++)
                profile[i] /= totalWeight;

            return profile;
        }
    }
}
