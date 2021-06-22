using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PracticalJwt.Application.Commands;
using PracticalJwt.Application.Dtos;
using PracticalJwt.Application.Queries;
using PracticalJwt.Domain.Models;
using System;
using System.Net.Mime;
using System.Threading.Tasks;

namespace PracticalJwt.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private string AdminRole;
        private string UserRole;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
            AdminRole = Enum.GetName(typeof(Role), Role.Admin);
            UserRole = Enum.GetName(typeof(Role), Role.User);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseDto>> Register(RegisterCommand command)
        {
            return new OkObjectResult(
                await _mediator.Send(command)
            );
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseDto>> Login(LoginCommand cmd)
        {
            return new OkObjectResult(
                await _mediator.Send(cmd)
            );
        }

        [AllowAnonymous]
        [HttpGet("newtoken")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseDto>> NewToken(RefreshTokenDto refreshToken)
        {
            var cmd = new GetNewTokenCommand()
            {
                AccessToken = refreshToken.AccessToken,
                RefreshToken = refreshToken.RefreshToken
            };

            return new OkObjectResult(await _mediator.Send(cmd));
        }

        [HttpPost("logout")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseDto>> Logout(LogoutCommand cmd)
        {
            return new OkObjectResult(
                await _mediator.Send(cmd)
            );
        }

        [HttpGet("getage")]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ResponseDto>> GetAge()
        {
            //get username from claims
            var userName = User.Identity?.Name;

            var query = new GetAgeQuery(userName);
            return new OkObjectResult(await _mediator.Send(query));
        }

        [Authorize(Roles = nameof(Role.Admin))] //by admins only
        [HttpGet("deactivate/{userId}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ResponseDto>> Deactivate(int userId)
        {
            return new OkObjectResult(
                await _mediator.Send(new DeactivateCommand() { UserID = userId })
            ) ;
        }
    }
}
