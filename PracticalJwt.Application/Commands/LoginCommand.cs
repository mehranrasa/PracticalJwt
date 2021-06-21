using MediatR;
using Newtonsoft.Json;
using PracticalJwt.Application.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace PracticalJwt.Application.Commands
{
    public class LoginCommand : IRequest<ResponseDto>
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, ResponseDto>
    {
        private readonly IUserService _userService;

        public LoginCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<ResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var result = await _userService.Login(request.Username, request.Password);

            if (result.InvalidUserPass)
                return response(false, null);

            //successfull login
            return response(succeed: true, result.Token);
        }

        private ResponseDto response(bool succeed, object result)
        {
            return new ResponseDto() { Succeed = succeed, Result = result };
        }
    }
}
