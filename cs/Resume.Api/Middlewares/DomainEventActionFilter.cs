using Data.Writer;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Domain.SharedKernel.DomainEvents;

namespace Resume.Api.Middlewares
{
    public class DomainEventActionFilter : IAsyncActionFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public DomainEventActionFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }


        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // pre action 
            await next();
            // post action
            var dbContext = _serviceProvider.GetService<IDbContext>();
            var dispatcher = _serviceProvider.GetService<IDomainEventDispatcher>();
            await dbContext.SaveChangesAsync();
            await dispatcher.DispatchAsync(_serviceProvider);
        }
    }
}
