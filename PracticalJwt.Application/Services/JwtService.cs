using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PracticalJwt.Application.Services
{
    public interface IJwtService
    {
        JwtTokenResult GenerateToken(string username);

        DecodedTokenResult DecryptToken(string accessToken);
    }


    public class JwtService : IJwtService
    {
        private byte[] _secret;
        private SymmetricSecurityKey _key;
        private SigningCredentials _signinCerd;
        private EncryptingCredentials _encryptCerd;

        private readonly IConfiguration _configuration;

        private readonly int _accessTokenExpirationMintues;
        private readonly int _refreshTokenExpirationDays;



        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
            _secret = Encoding.ASCII.GetBytes(_configuration["JWTConfig:Secret"]);
            _accessTokenExpirationMintues = Convert.ToInt32(_configuration["JWTConfig:AccessTokenExpirationMinutes"]);
            _refreshTokenExpirationDays = Convert.ToInt32(_configuration["JWTConfig:RefreshTokenExpirationDays"]);

            _key = new SymmetricSecurityKey(_secret);
            _signinCerd = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
            _encryptCerd = new EncryptingCredentials(_key, SecurityAlgorithms.Aes256KW, SecurityAlgorithms.Aes256CbcHmacSha512);
        }

        public JwtTokenResult GenerateToken(string username)
        {
            return new JwtTokenResult()
            {
                AccessToken = generateAccessToken(username),
                RefreshToken = generateRefreshToken(username),
                RefreshTokenExpirationTime = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays)
            };
        }

        public DecodedTokenResult DecryptToken(string accessToken)
        {
            try
            {
                var claims = new JwtSecurityTokenHandler().ValidateToken(accessToken,
                    new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidIssuer = null,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = _key,
                        TokenDecryptionKey = _key,
                        ValidAudience = null,
                        ValidateAudience = false,
                        ValidateLifetime = false,
                        ClockSkew = TimeSpan.FromSeconds(1)
                    }, out var validatedToken);

                return new DecodedTokenResult()
                {
                    Claims = claims,
                    Token = validatedToken,
                    Username = claims.Identity.Name
                };
            }
            catch(Exception e)
            {
                var a = e;
                return null;
            }
        }


        private string generateAccessToken(string username)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username)/*,
                new Claim(ClaimTypes.Role, "")*/
            };

            //encrypted jwt
            var jwtSecurityToken = new JwtSecurityTokenHandler().CreateJwtSecurityToken(
                issuer: null,
                audience: null,
                subject: new ClaimsIdentity(claims),
                notBefore: null,
                expires: DateTime.UtcNow.AddMinutes(_accessTokenExpirationMintues),
                issuedAt: null,
                signingCredentials: _signinCerd,
                encryptingCredentials: _encryptCerd
            );

            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return token;
        }

        private string generateRefreshToken(string username)
        {
            var guid = Guid.NewGuid().ToString();
            var refreshToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(username + guid));
            return refreshToken;
        }

    }

    public class DecodedTokenResult
    {
        public string Username { get; set; }

        public ClaimsPrincipal Claims { get; set; }

        public SecurityToken Token { get; set; }
    }

    public class JwtTokenResult
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public DateTime RefreshTokenExpirationTime { get; set; }
    }
}
