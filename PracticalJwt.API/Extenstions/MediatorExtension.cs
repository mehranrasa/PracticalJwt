using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PracticalJwt.Application.Commands;
using System.Reflection;

namespace PracticalJwt.API.Extenstions
{
    public static class MediatorExtension
    {
        public static IServiceCollection AddMediatr(this IServiceCollection services)
        {
            var applicationAssembly = typeof(LoginCommand).Assembly;
            services.AddMediatR(Assembly.GetExecutingAssembly(), applicationAssembly);
            return services;
        }
    }
}
