using MediatR;
using PracticalJwt.Application.Dtos;
using PracticalJwt.Application.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PracticalJwt.Application.Commands
{
    public class GetNewTokenCommand : IRequest<ResponseDto>
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public class GetNewTokenCommandHandler : IRequestHandler<GetNewTokenCommand, ResponseDto>
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;

        public GetNewTokenCommandHandler(IUserService userService, IJwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        public async Task<ResponseDto> Handle(GetNewTokenCommand request, CancellationToken cancellationToken)
        {
            if (String.IsNullOrEmpty(request.AccessToken) || String.IsNullOrEmpty(request.RefreshToken))
                return new ResponseDto() { Succeed = false, Result = null };

            var tokens = await _userService.RefreshToken(request.AccessToken, request.RefreshToken);

            return new ResponseDto()
            {
                Succeed = !(tokens == null),
                Result = tokens
            };
        }
    }
}
