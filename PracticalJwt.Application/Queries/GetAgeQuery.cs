using MediatR;
using PracticalJwt.Application.Dtos;
using PracticalJwt.Application.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace PracticalJwt.Application.Queries
{
    public class GetAgeQuery : IRequest<ResponseDto>
    {
        public string Username { get; set; }

        public GetAgeQuery(string username)
        {
            Username = username;
        }
    }

    public class GetAgeQueryHandler : IRequestHandler<GetAgeQuery, ResponseDto>
    {
        private readonly IUserRepository _userRepository;

        public GetAgeQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ResponseDto> Handle(GetAgeQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.Get(request.Username);
            return new ResponseDto()
            {
                Succeed = user != null,
                Result = user.Age
            };
        }
    }
}