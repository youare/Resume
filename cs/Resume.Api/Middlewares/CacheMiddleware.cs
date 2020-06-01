using Cache;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Resume.Api.Middlewares
{
    public static class CacheMiddlewareExtensions
    {
        public static IServiceCollection ConfigureCache(this IServiceCollection servcies)
        {
            return servcies.AddMemoryCache().AddSingleton<ICache, Cache.Cache>();
        }
    }
}
