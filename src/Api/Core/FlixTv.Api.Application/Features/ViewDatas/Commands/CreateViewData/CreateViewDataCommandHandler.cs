using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models.RequestModels.ViewDatas;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.ViewDatas.Commands.CreateViewData
{
    public class CreateViewDataCommandHandler : IRequestHandler<CreateViewDataCommandRequest, Unit>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public CreateViewDataCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Unit> Handle(CreateViewDataCommandRequest request, CancellationToken cancellationToken)
        {
            if (await unitOfWork.GetReadRepository<User>().GetAsync(u => u.Id == request.UserId) is null)
                throw new Exception("User was not found");

            if (await unitOfWork.GetReadRepository<Movie>().GetAsync(m => m.Id == request.MovieId) is null)
                throw new Exception("Movie was not found");

            var viewData = new ViewData(request.UserId, request.MovieId);

            await unitOfWork.GetWriteRepository<ViewData>().AddAsync(viewData);

            await unitOfWork.SaveAsync();

            return Unit.Value;
        }
    }
}
