using FlixTv.Api.Application.Interfaces.AutoMapper;
using FlixTv.Api.Application.Interfaces.UnitOfWorks;
using FlixTv.Api.Domain.Concretes;
using FlixTv.Common;
using FlixTv.Common.Infrastructure;
using FlixTv.Common.Models.DTOs;
using FlixTv.Common.Models.RequestModels.Auth;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Api.Application.Features.Auth.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommandRequest, Unit>
    {
        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;

        public RegisterCommandHandler(IMapper mapper, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task<Unit> Handle(RegisterCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.Email);

            if (user is not null)
                throw new Exception("User with this email already exists.");

            user = mapper.Map<User, RegisterCommandRequest>(request);
            user.UserName = request.Email;
            user.SecurityStamp = Guid.NewGuid().ToString();

            var result = await userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                if (!await roleManager.RoleExistsAsync("User"))
                    await roleManager.CreateAsync(new Role
                    {
                        Name = "User",
                        NormalizedName = "USER",
                    });

                await userManager.AddToRoleAsync(user, "User");

                var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                var param = new Dictionary<string, string>
                {
                    { "token", token },
                    { "email", user.Email }
                };

                var callback = QueryHelpers.AddQueryString(request.ClientUri!, param);

                QueueFactory.SendMessageToExchange(
                exchangeName: FlixTvConstants.MovieExchangeName,
                exchangeType: FlixTvConstants.DefaultExchangeType,
                queueName: FlixTvConstants.SendEmailQueueName,
                obj: new EmailMessageDto
                {
                    To = request.Email,
                    Subject = "Email Confirmation Token",
                    Body = callback
                });
            }


            return Unit.Value;
        }
    }
}
