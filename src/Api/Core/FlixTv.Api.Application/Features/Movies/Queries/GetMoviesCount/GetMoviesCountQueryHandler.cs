using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Movies.Queries.GetMoviesCount
{
    public class GetMoviesCountQueryHandler : IRequestHandler<GetMoviesCountQueryRequest, int>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GetMoviesCountQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<int> Handle(GetMoviesCountQueryRequest request, CancellationToken cancellationToken)
        {
            var response = await unitOfWork.GetReadRepository<Movie>().CountAsync(request.predicate);

            return response;
        }
    }
}
