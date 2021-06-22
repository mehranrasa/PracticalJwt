using MediatR;
using PracticalJwt.Application.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace PracticalJwt.Application.Commands
{
    public class DeactivateCommand : IRequest<ResponseDto>
    {
        public int UserID { get; set; }
    }

    public class DeactivateCommandHandler : IRequestHandler<DeactivateCommand, ResponseDto>
    {
        private readonly IUserService _userService;

        public DeactivateCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<ResponseDto> Handle(DeactivateCommand request, CancellationToken cancellationToken)
        {
            var res = await _userService.Deactivate(request.UserID);
            return new ResponseDto()
            {
                Succeed = res,
                Result = res
            };
        }
    }
}
