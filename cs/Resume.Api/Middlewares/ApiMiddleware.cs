using CorrelationId;
using Data.Writer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Resume.Api.Controllers;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace Resume.Api.Middlewares
{
    public class ApiMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, IServiceProvider serviceProvider)
        {
            var username = BaseController.GetCurrentUser(httpContext);
            var context = serviceProvider.GetService<IDbContext>();
            using (LogContext.PushProperty("username", username))
            {
                try
                {
                    await _next(httpContext);
                    await context.SaveChangesAsync();

                } 
                catch (Exception ex)
                {
                    await HandleExceptionAsync(httpContext, ex);
                }
                finally
                {

                }
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var result = ex.Message;
            return context.Response.WriteAsync(JsonConvert.SerializeObject(result, Formatting.None, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
        }
    }
    public static class ApiMiddlewareExtensions
    {
        public static void ConfigureCustomHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<ApiMiddleware>();
        }
    }
}
