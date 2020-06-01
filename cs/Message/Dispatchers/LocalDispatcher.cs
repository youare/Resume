using BusinessLayer.Command.CommandHandlers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Message.Dispatchers
{
    public interface IMessageBus
    {
        ConcurrentQueue<IMessage> Queue { get; }
    }
    public class InMemoryMessageBus: IMessageBus
    {

        public ConcurrentQueue<IMessage> Queue { get; }
        public InMemoryMessageBus()
        {
            Queue = new ConcurrentQueue<IMessage>();
        }
                
    }
    public class LocalDispatcher : IDispatcher
    {
        private readonly IMessageBus _messageBus;

        public LocalDispatcher(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        public Task DispatchAsync(IMessage message)
        {
            _messageBus.Queue.Enqueue(message);
            return Task.CompletedTask;
        }

        public Task DispatchAsync<TData>(IMessage<TData> message)
        {
            _messageBus.Queue.Enqueue(message);
            return Task.CompletedTask;
        }

        public Task DispatchAsync<TData>(ICommand<TData> command)
        {
            _messageBus.Queue.Enqueue(command);
            return Task.CompletedTask;
        }
    }
}
