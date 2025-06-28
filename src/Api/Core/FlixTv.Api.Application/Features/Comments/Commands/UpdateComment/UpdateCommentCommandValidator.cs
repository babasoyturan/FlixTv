using FlixTv.Common.Models.RequestModels.Comments;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Comments.Commands.UpdateComment
{
    public class UpdateCommentCommandValidator : AbstractValidator<UpdateCommentCommandRequest>
    {
        public UpdateCommentCommandValidator()
        {
            RuleFor(c => c.Message)
                .NotEmpty()
                .MaximumLength(1000);
        }
    }
}
