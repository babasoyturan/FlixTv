using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.ViewDatas.Commands.DeleteViewData
{
    public class DeleteViewDataCommandHandler : IRequestHandler<DeleteViewDataCommandRequest, Unit>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public DeleteViewDataCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteViewDataCommandRequest request, CancellationToken cancellationToken)
        {
            var viewData = await unitOfWork.GetReadRepository<ViewData>().GetAsync(c => c.Id == request.ViewDataId);

            if (viewData is null)
                throw new Exception("View Data was not found.");

            await unitOfWork.GetWriteRepository<ViewData>().DeleteAsync(viewData);
            await unitOfWork.SaveAsync();

            return Unit.Value;
        }
    }
}
