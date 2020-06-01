using BusinessLayer.Command.CommandHandlers;
using Message.Dispatchers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Resume.Api.Workers
{
    public class SubscriptionWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMessageBus _messageBus;
        private readonly IConfigManager _configManager;
        private readonly ILogger<SubscriptionWorker> _logger;

        public SubscriptionWorker(IServiceProvider serviceProvider, IMessageBus messageBus, IConfigManager configManager, ILogger<SubscriptionWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _messageBus = messageBus;
            _configManager = configManager;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        if(_messageBus.Queue.TryDequeue(out var message))
                        {
                            if (message is ICommand command)
                            {
                                var commandHandler = scope
                                    .ServiceProvider
                                    .GetServices(typeof(ICommandHandler<,>))
                                    .FirstOrDefault(x => x.GetType().GetGenericArguments()[1] == command.GetType());
                                await ((dynamic)commandHandler).HandleAsync(command);
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                   
                }
                finally
                {
                    await Task.Delay(100);
                }
            }
        }
    }
}
