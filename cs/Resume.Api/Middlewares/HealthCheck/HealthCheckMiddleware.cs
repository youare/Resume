using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Resume.Api.Middlewares.HealthCheck
{
    public static class HealthCheckMiddlewareExtensions
    {
        public static IServiceCollection ConfigureHealthCheck(this IServiceCollection services)
        {
            var configManager = services.BuildServiceProvider().GetService<IConfigManager>();
            services.AddHealthChecks()
                .AddCheck<MemoryHealthCheck>("Memory");
                //.AddCheck<ServerCpuHealthCheck>("ServerCpu")
                //.AddCheck<ServerMemoryHealthCheck>("ServerMemory")
                //.AddCheck<ServerDiskHealthCheck>("ServerDisk");
            services.AddSingleton<IHealthCheckPublisher, HealthCheckPublisher>();
            return services;
        }
        public static void ConfigureHealthCheckEndpoint(this IEndpointRouteBuilder endPoints)
        {
            endPoints.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions { 
                ResponseWriter = (httpContext, result) => httpContext.Response.WriteAsync(JsonConvert.SerializeObject(result)),
                ResultStatusCodes =
                {
                    [HealthStatus.Degraded] = (int)HttpStatusCode.OK,
                    [HealthStatus.Unhealthy] = (int)HttpStatusCode.OK,
                    [HealthStatus.Healthy] = (int)HttpStatusCode.OK,
                }
            });
        }
    }
}
