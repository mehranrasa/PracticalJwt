using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace PracticalJwt.API.Extenstions
{
    public static class JwtExtension
    {
        public static IServiceCollection AddJwtBearer(this IServiceCollection services,
            IConfiguration configuration)
        {
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["JWTConfig:Secret"]));
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = key,
                    TokenDecryptionKey = key,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ClockSkew = TimeSpan.FromSeconds(1),
                };
            });

            return services;
        }
    }
}
