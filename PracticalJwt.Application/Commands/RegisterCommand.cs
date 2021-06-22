using MediatR;
using Newtonsoft.Json;
using PracticalJwt.Application.Dtos;
using PracticalJwt.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace PracticalJwt.Application.Commands
{
    public class RegisterCommand : IRequest<ResponseDto>
    {
        [JsonProperty("username")]
        [Required]
        public string Username { get; set; }

        [JsonProperty("password")]
        [Required]
        public string Password { get; set; }

        [JsonProperty("age")]
        public int Age { get; set; }

        [JsonProperty("role")]
        public int Role { get; set; }
    }

    public class RegistrationCommandHandler : IRequestHandler<RegisterCommand, ResponseDto>
    {
        private readonly IUserService _userService;

        public RegistrationCommandHandler(IUserService userService)
        {
            _userService = userService;
        }


        public async Task<ResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (!isValid(request.Username, request.Password))
                return response(false, "invalid username/password");

            //register
            var createdUser = await _userService.Register(new User()
            {
                Username = request.Username,
                Password = request.Password,
                Age = request.Age,
                UserRole = (Role)request.Role
            });

            if (createdUser == null)
                return response(false, "could not create user");

            //automatic login after registration
            var res = await _userService.Login(createdUser);

            return response(true, res.Token);
        }

        private ResponseDto response(bool succeed, object result)
        {
            return new ResponseDto() { Succeed = succeed, Result = result };
        }

        private bool isValid(string username, string password)
        {
            return !(string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(password) ||
                username.Length > 50 ||
                password.Length > 255);
        }
    }
}
