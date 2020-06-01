using BusinessLayer.Command.DomainErrorHandlers;
using Data.Writer;
using Domain.SharedKernel.DomainEvents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Command.DomainEvent
{
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly ConcurrentQueue<IDomainEvent> _queue;
        private readonly ILogger<DomainEventDispatcher> _logger;
        private readonly List<Type> _handlers;

        public DomainEventDispatcher(ILogger<DomainEventDispatcher> logger)
        {
            _logger = logger;
            _queue = new ConcurrentQueue<IDomainEvent>();
            _handlers = Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => x.GetInterfaces().Any(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(IDomainEvent)))
                .ToList();
        }

        public void Enqueue(IDomainEvent domainEvent)
        {
            _queue.Enqueue(domainEvent);
        }
        public async Task DispatchAsync(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                while (_queue.Count > 0)
                {

                    if (_queue.TryDequeue(out var domainEvent))
                    {
                        foreach (var handlerType in _handlers)
                        {
                            var canHandle = handlerType.GetInterfaces()
                                .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>));
                            if (canHandle)
                            {
                                var handler = Activator.CreateInstance(handlerType);
                                var method = handler.GetType().GetMethod(nameof(IDomainEventHandler<IDomainEvent>.HandleAsync));
                                var task = (Task)method.Invoke(handler, new object[] { domainEvent, scope.ServiceProvider });
                                await task.ConfigureAwait(false);
                                var context = scope.ServiceProvider.GetService<IDbContext>();
                                await context.SaveChangesAsync();
                            }
                        }
                    }
                    else
                    {

                    }
                }
            }
        }
    }
}
