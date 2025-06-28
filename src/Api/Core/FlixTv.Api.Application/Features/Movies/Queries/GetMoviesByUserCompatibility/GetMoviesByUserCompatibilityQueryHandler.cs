using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Application.Utilities;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models;
using FlixTv.Common.Models.ResponseModels.Movies;
using MediatR;
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

        private static readonly List<MovieCategory> _allCategories = Enum.GetValues(typeof(MovieCategory)).Cast<MovieCategory>().ToList();
        private const int _minReleaseYear = 1900;
        private const int _maxReleaseYear = 3000;
        private static readonly double _yearRange = (_maxReleaseYear - _minReleaseYear) > 0 ? (_maxReleaseYear - _minReleaseYear) : 1;

        public GetMoviesByUserCompatibilityQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<IList<GetAllMoviesQueryResponse>> Handle(GetMoviesByUserCompatibilityQueryRequest request, CancellationToken cancellationToken)
        {
            var user = await unitOfWork.GetReadRepository<User>().GetAsync(
                u => u.Id == request.userId, 
                x => x
                .Include(u => u.WatchedHistory).ThenInclude(vd => vd.Movie)
                .Include(u => u.Reviews).ThenInclude(r => r.Movie)
                .Include(u => u.FavoriteMovies).ThenInclude(fm => fm.Movie));

            if (user is null)
                throw new Exception("The user was not found");

            var userVector = BuildUserProfile(user);

            var relatedMovies = await unitOfWork.GetReadRepository<Movie>()
                .GetAllByPagingAsync(
                predicate: request.predicate.And(m => m.IsVisible && !user.WatchedHistory.Any(wh => wh.MovieId == m.Id)),
                currentPage: request.currentPage, pageSize: request.pageSize,
                orderBy: x => x.OrderByDescending(m => CosineSimilarity(userVector, m.FeatureVector)),
                include: x => x.Include(m => m.Reviews).Include(m => m.Views));

            if (relatedMovies.Count() <= 0)
                throw new Exception("There are not any related movie.");

            var response = mapper.Map<GetAllMoviesQueryResponse, Movie>(relatedMovies);

            for (int i = 0; i < response.Count(); i++)
                response[0].ViewCount = relatedMovies[0].Views.Count();

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
