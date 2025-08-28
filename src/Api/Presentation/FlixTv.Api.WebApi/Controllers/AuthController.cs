using FlixTv.Common.Models.RequestModels.Auth;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FlixTv.Api.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator mediator;

        public AuthController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> EmailConfirmation([FromQuery] string Email, [FromQuery] string Token)
        {
            var request = new EmailConfirmationCommandRequest
            {
                Email = Email,
                Token = Token
            };

            await mediator.Send(request);

            return StatusCode(StatusCodes.Status200OK, "Email was confirmed successfully");
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterCommandRequest request)
        {
            await mediator.Send(request);

            return StatusCode(StatusCodes.Status201Created, "User was created successfully");
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginCommandRequest request)
        {
            var response = await mediator.Send(request);

            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpPost]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommandRequest request)
        {
            var response = await mediator.Send(request);

            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpPost]
        public async Task<IActionResult> Revoke([FromBody] RevokeCommandRequest request)
        {
            await mediator.Send(request);

            return StatusCode(StatusCodes.Status200OK);
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommandRequest request)
        {
            await mediator.Send(request);

            return StatusCode(StatusCodes.Status200OK, "Confirmation link was sent to your email.");
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommandRequest request)
        {
            await mediator.Send(request);

            return StatusCode(StatusCodes.Status200OK, "Password was reset successfully");
        }
    }
}