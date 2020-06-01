using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Resume.Api.Middlewares
{
    public static class AuthenticationMiddlewareExtensions
    {
        public static IServiceCollection ConfigureAuthentication(this IServiceCollection services)
        {
            var configManager = services.BuildServiceProvider().GetService<IConfigManager>();
            services.AddAuthentication("Bearer").AddJwtBearer("Bearer", config =>
            {
                config.Authority = configManager.AppConfig.UriConfig.Authority.Address;
                config.Audience = configManager.AppConfig.ApplicationName;
            });
            return services;
        }
    }
}
