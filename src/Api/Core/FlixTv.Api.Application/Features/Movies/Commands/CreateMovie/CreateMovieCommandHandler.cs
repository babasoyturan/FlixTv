using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common;
using FlixTv.Common.Infrastructure;
using FlixTv.Common.Models.DTOs;
using FlixTv.Common.Models.RequestModels.Movies;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Movies.Commands.CreateMovie
{
    public class CreateMovieCommandHandler : IRequestHandler<CreateMovieCommandRequest, Unit>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public CreateMovieCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Unit> Handle(CreateMovieCommandRequest request, CancellationToken cancellationToken)
        {
            var movie = mapper.Map<Movie, CreateMovieCommandRequest>(request);

            //var coverImageId = Guid.NewGuid().ToString();
            //var bannerImageId = Guid.NewGuid().ToString();

            //movie.CoverImageUrl = $"https://{FlixTvConstants.CdnName}.cloudfront.net/images/{coverImageId}.png";
            //movie.BannerImageUrl = $"https://{FlixTvConstants.CdnName}.cloudfront.net/images/{bannerImageId}.png";

            movie.SetFeatureVector();

            if (request.InitialRating.HasValue)
                movie.Rating = request.InitialRating.Value;

            if (request.TmdbId.HasValue)
                movie.TmdbId = request.TmdbId.Value;

            var allMovies = await unitOfWork.GetReadRepository<Movie>()
                .GetAllAsync(include: x => x.Include(m => m.SimilarMovies));

            await unitOfWork.GetWriteRepository<Movie>().AddAsync(movie);
            await unitOfWork.SaveAsync();



            double similarityThreshold = 0.9;

            if (allMovies != null)
                foreach (var m in allMovies)
                {
                    double score = CosineSimilarity(movie.FeatureVector, m.FeatureVector);

                    if (score >= similarityThreshold)
                    {
                        if (movie.SimilarMovies == null)
                            movie.SimilarMovies = new List<Movie>();

                        movie.SimilarMovies.Add(m);

                        if (m.SimilarMovies == null)
                            m.SimilarMovies = new List<Movie>();

                        m.SimilarMovies.Add(movie);

                        await unitOfWork.GetWriteRepository<Movie>().UpdateAsync(m);
                    }
                }

            await unitOfWork.GetWriteRepository<Movie>().UpdateAsync(movie);
            await unitOfWork.SaveAsync();


            //////////////////////////////////////////////////////////////////


            //var coverBytes = await ToByteArray(request.CoverImage);
            //var bannerBytes = await ToByteArray(request.BannerImage);

            //QueueFactory.SendMessageToExchange(
            //    exchangeName: FlixTvConstants.MovieExchangeName,
            //    exchangeType: FlixTvConstants.DefaultExchangeType,
            //    queueName: FlixTvConstants.UploadImageQueueName,
            //    obj: new FormFileDto { 
            //        Key = coverImageId, 
            //        FileName = request.CoverImage.FileName, 
            //        ContentType = request.CoverImage.ContentType, 
            //        FileData = coverBytes 
            //    });

            //QueueFactory.SendMessageToExchange(
            //    exchangeName: FlixTvConstants.MovieExchangeName,
            //    exchangeType: FlixTvConstants.DefaultExchangeType,
            //    queueName: FlixTvConstants.UploadImageQueueName,
            //    obj: new FormFileDto { 
            //        Key = bannerImageId, 
            //        FileName = request.BannerImage.FileName, 
            //        ContentType = request.BannerImage.ContentType, 
            //        FileData = bannerBytes
            //    });


            return Unit.Value;
        }

        async Task<byte[]> ToByteArray(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            return ms.ToArray();
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
