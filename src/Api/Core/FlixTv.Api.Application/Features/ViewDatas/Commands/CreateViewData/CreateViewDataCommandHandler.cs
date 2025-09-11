using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.RequestModels.ViewDatas;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.ViewDatas.Commands.CreateViewData
{
    public class CreateViewDataCommandHandler : IRequestHandler<CreateViewDataCommandRequest, Unit>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly int userId;

        public CreateViewDataCommandHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            this.unitOfWork = unitOfWork;
            this.userId = Convert.ToInt32(httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        public async Task<Unit> Handle(CreateViewDataCommandRequest request, CancellationToken cancellationToken)
        {
            if (await unitOfWork.GetReadRepository<User>().GetAsync(u => u.Id == userId) is null)
                throw new Exception("User was not found");


            var movie = await unitOfWork.GetReadRepository<Movie>().GetAsync(m => m.Id == request.MovieId);

            if (movie is null)
                throw new Exception("Movie was not found");

            var vd = await unitOfWork.GetReadRepository<ViewData>().GetAsync(x => x.UserId == userId && x.MovieId == request.MovieId);

            var now = DateTime.UtcNow;

            if (vd is null)
            {
                vd = new ViewData(userId, request.MovieId)
                {
                    LastPositionSeconds = request.LastPositionSeconds,
                    MaxPositionSeconds = request.MaxPositionSeconds,
                    WatchedSeconds = request.WatchedSeconds,
                    LastWatchedAt = now,
                };

                if ((((movie.TmdbId.HasValue
                    && movie.TmdbId.Value <= 0)
                    || !movie.TmdbId.HasValue)
                    && request.WatchedSeconds >= movie.Duration * 60 * 0.7
                    && request.MaxPositionSeconds >= movie.Duration * 60 * 0.8)
                    || (movie.TmdbId.HasValue
                    && movie.TmdbId.Value > 0
                    && request.WatchedSeconds >= movie.Duration * 60 * 0.9))
                    vd.IsCompleted = true;

                await unitOfWork.GetWriteRepository<ViewData>().AddAsync(vd);
            }
            else
            {
                vd.LastPositionSeconds = request.LastPositionSeconds;
                vd.MaxPositionSeconds = Math.Max(vd.MaxPositionSeconds, request.MaxPositionSeconds);
                vd.WatchedSeconds += request.WatchedSeconds;

                vd.LastWatchedAt = now;

                if (!vd.IsCompleted 
                    && ((((movie.TmdbId.HasValue 
                    && movie.TmdbId.Value <= 0)
                    || !movie.TmdbId.HasValue)
                    && vd.WatchedSeconds >= movie.Duration * 60 * 0.7 
                    && vd.MaxPositionSeconds >= movie.Duration * 60 * 0.8)
                    || (movie.TmdbId.HasValue
                    && movie.TmdbId.Value > 0
                    && vd.WatchedSeconds >= movie.Duration * 60 * 0.9)))
                    vd.IsCompleted = true;

                await unitOfWork.GetWriteRepository<ViewData>().UpdateAsync(vd);
            }

            await unitOfWork.SaveAsync();
            return Unit.Value;
        }
    }
}
