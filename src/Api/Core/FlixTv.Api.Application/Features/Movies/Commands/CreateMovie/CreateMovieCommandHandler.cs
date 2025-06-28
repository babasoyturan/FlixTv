using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common;
using FlixTv.Common.Infrastructure;
using FlixTv.Common.Models.DTOs;
using FlixTv.Common.Models.RequestModels.Movies;
using MediatR;
using Microsoft.AspNetCore.Http;
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
            var coverImageId = Guid.NewGuid().ToString();
            var bannerImageId = Guid.NewGuid().ToString();

            var movie = mapper.Map<Movie, CreateMovieCommandRequest>(request);

            movie.CoverImageUrl = coverImageId;
            movie.BannerImageUrl = bannerImageId;
            movie.SetFeatureVector();
            movie.SetMovieRating();

            await unitOfWork.GetWriteRepository<Movie>().AddAsync(movie);
            await unitOfWork.SaveAsync();


            var coverBytes = await ToByteArray(request.CoverImage);
            var bannerBytes = await ToByteArray(request.BannerImage);

            QueueFactory.SendMessageToExchange(
                exchangeName: FlixTvConstants.MovieExchangeName,
                exchangeType: FlixTvConstants.DefaultExchangeType,
                queueName: FlixTvConstants.UploadImageQueueName,
                obj: new FormFileDto { 
                    Key = coverImageId, 
                    FileName = request.CoverImage.FileName, 
                    ContentType = request.CoverImage.ContentType, 
                    FileData = coverBytes 
                });

            QueueFactory.SendMessageToExchange(
                exchangeName: FlixTvConstants.MovieExchangeName,
                exchangeType: FlixTvConstants.DefaultExchangeType,
                queueName: FlixTvConstants.UploadImageQueueName,
                obj: new FormFileDto { 
                    Key = bannerImageId, 
                    FileName = request.BannerImage.FileName, 
                    ContentType = request.BannerImage.ContentType, 
                    FileData = bannerBytes
                });


            return Unit.Value;
        }

        async Task<byte[]> ToByteArray(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            return ms.ToArray();
        }
    }
}
