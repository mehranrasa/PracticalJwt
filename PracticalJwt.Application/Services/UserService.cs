using PracticalJwt.Application.Helpers;
using PracticalJwt.Application.Repositories;
using PracticalJwt.Application.Services;
using PracticalJwt.Domain.Models;
using System;
using System.Threading.Tasks;

namespace PracticalJwt.Application
{
    public interface IUserService
    {
        Task<LoginResult> Login(string username, string password);

        Task<LoginResult> Login(User user);

        Task<bool> Logout(string token);

        Task<User> Register(User user);

        Task<bool> Deactivate(int userId);

        Task<TokenResult> RefreshToken(string accessToken, string refreshToken);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly PasswordEncryptor pwHelper = new PasswordEncryptor();

        public UserService(IUserRepository userRepository, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }


        public async Task<User> Register(User user)
        {
            //encrypt password
            user.Password = pwHelper.Encrypt(user.Password);
            user.IsActive = true;
            var createdUser = await _userRepository.Create(user);
            return createdUser;
        }

        public async Task<LoginResult> Login(string username, string password)
        {
            var user = await _userRepository.Get(username);

            //validate username and password
            if ((user == null) || (!user.Password.Equals(pwHelper.Encrypt(password))))
                return new LoginResult() { InvalidUserPass = true, Token = null };

            //generate a token
            var token = _jwtService.GenerateToken(user.Username, user.UserRole);

            if (await _userRepository.UpdateRefreshToken(user, token.RefreshToken, token.RefreshTokenExpirationTime))
            {
                return new LoginResult()
                {
                    InvalidUserPass = false,
                    Token = new TokenResult()
                    {
                        AccessToken = token.AccessToken,
                        RefreshToken = token.RefreshToken
                    }
                };
            }

            return null;
        }

        //login without check
        public async Task<LoginResult> Login(User user)
        {
            var token = _jwtService.GenerateToken(user.Username, user.UserRole);

            //save refreshtoken
            await _userRepository.UpdateRefreshToken(user, token.RefreshToken, token.RefreshTokenExpirationTime);

            return new LoginResult()
            {
                InvalidUserPass = false,
                Token = new TokenResult()
                {
                    AccessToken = token.AccessToken,
                    RefreshToken = token.RefreshToken
                }
            };
        }

        public async Task<TokenResult> RefreshToken(string accessToken, string refreshToken)
        {
            //decrypt/validate access token
            var decryptedToken = _jwtService.DecryptToken(accessToken);
            if (decryptedToken == null)
                return null;

            //validate refresh token
            var user = await _userRepository.Get(decryptedToken.Username, loadRefreshTokens: true);
            var currentUserRefreshToken = user.RefreshToken;
            if
            (
                (user == null) ||
                (!currentUserRefreshToken.Token.Equals(refreshToken)) ||
                (currentUserRefreshToken.ExpiresAt <= DateTime.UtcNow)
            )
                return null;

            //generate new tokens
            var tokens = _jwtService.GenerateToken(decryptedToken.Username, user.UserRole);

            //update the user's refresh token
            if (await _userRepository.UpdateRefreshToken(user, tokens.RefreshToken, tokens.RefreshTokenExpirationTime))
            {
                return new TokenResult()
                {
                    AccessToken = tokens.AccessToken,
                    RefreshToken = tokens.RefreshToken
                };
            }

            return null;
        }

        public Task<bool> Logout(string token)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Deactivate(int userId)
        {
            var user = await _userRepository.Get(userId);
            if (user == null)
                return false;

            user.IsActive = false;
            return await _userRepository.Update(user);
        }
    }


    public class TokenResult
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }


    public class LoginResult
    {
        //invalid username and/or password
        public bool InvalidUserPass { get; set; }

        public TokenResult Token { get; set; }
    }
}
