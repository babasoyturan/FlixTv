using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common.Models;
using FlixTv.Common.Models.DTOs;
using FlixTv.Common.Models.ResponseModels.Comments;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Comments.Queries.GetAllComments
{
    public class GetAllCommentsQueryHandler : IRequestHandler<GetAllCommentsQueryRequest, IList<GetCommentQueryResponse>>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly int userId;

        public GetAllCommentsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.userId = Convert.ToInt32(httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }

        public async Task<IList<GetCommentQueryResponse>> Handle(GetAllCommentsQueryRequest request, CancellationToken cancellationToken)
        {
            IList<Comment> comments = new List<Comment>();
            if (request.currentPage > 0 && request.pageSize > 0)
                comments = await unitOfWork.GetReadRepository<Comment>().GetAllByPagingAsync(
                    request.predicate,
                    request.orderBy,
                    request.currentPage,
                    request.pageSize,
                    x => x.Include(c => c.Author).Include(c => c.Movie));
            else
                comments = await unitOfWork.GetReadRepository<Comment>().GetAllAsync(
                    request.predicate,
                    request.orderBy,
                    x => x.Include(c => c.Author).Include(c => c.Movie));

            mapper.Map<MovieDto, Movie>(new Movie());
            mapper.Map<AuthorDto, User>(new User());

            var response = mapper.Map<GetCommentQueryResponse, Comment>(comments);


            for (int i = 0; i < response.Count(); i++)
            {
                response[i].LikeCount = comments[i].Likes.Count;
                response[i].DislikeCount = comments[i].Dislikes.Count;
                response[i].HasLiked = userId == 0 ? false : comments[i].Likes.Contains(userId);
                response[i].HasDisliked = userId == 0 ? false : comments[i].Dislikes.Contains(userId);
            }

            return response;
        }
    }
}
