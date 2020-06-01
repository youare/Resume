using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Resume.Api.Middlewares
{
    public static class AuthorizationMiddlewareExtensions 
    {
        public static IServiceCollection ConfigureAuthorization(this IServiceCollection services)
        {
            return services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyConstants.ResumeApiReadOnly, p => p.RequireClaim(ClaimConstants.ResumeApiReadOnly, "true"));
            });
        }
    }

    public static class PolicyConstants
    {
        public const string ResumeApiReadOnly = "ResumeApiReadOnly";
    }
    public static class ClaimConstants
    {
        public const string ResumeApiReadOnly = "ResumeApiReadOnly";
    }
}
