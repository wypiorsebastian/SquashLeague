using Microsoft.Extensions.DependencyInjection;
using SquashLeague.Application.Contracts;
using SquashLeague.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquashLeague.Infrastructure
{
    public static class IoC
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddSingleton<IEmailService, EmailService>();

            return services;
        }
    }
}
