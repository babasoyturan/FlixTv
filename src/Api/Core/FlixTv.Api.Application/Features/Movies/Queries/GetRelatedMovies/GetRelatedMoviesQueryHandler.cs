using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.ResponseModels.Movies;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Movies.Queries.GetRelatedMovies
{
    public class GetRelatedMoviesQueryHandler : IRequestHandler<GetRelatedMoviesQueryRequest, IList<GetAllMoviesQueryResponse>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        private const double wCat = 1.0;
        private const double wYear = 0.1;
        private const double wAge = 0.2;

        public GetRelatedMoviesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<IList<GetAllMoviesQueryResponse>> Handle(GetRelatedMoviesQueryRequest request, CancellationToken cancellationToken)
        {
            var movie = await unitOfWork.GetReadRepository<Movie>().GetAsync(m => m.Id == request.MovieId);

            if (movie is null)
                throw new Exception("The movie is not found.");

            var relatedMovies = await unitOfWork.GetReadRepository<Movie>()
                .GetAllByPagingAsync(
                predicate: m => m.IsVisible && m.Id != movie.Id, 
                currentPage: 1, pageSize: request.Size,
                orderBy: x => x.OrderByDescending(m => CosineSimilarity(movie.FeatureVector, m.FeatureVector)),
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
    }
}
