using MediatR;
using Newtonsoft.Json;
using PracticalJwt.Application.Dtos;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PracticalJwt.Application.Commands
{
    public class LogoutCommand : IRequest<ResponseDto>
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }

    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, ResponseDto>
    {
        public Task<ResponseDto> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}


