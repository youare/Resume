using BusinessLayer.Command.CommandHandlers;
using System;
using System.Threading.Tasks;

namespace Message
{
    public interface IDispatcher
    {
        public Task DispatchAsync(IMessage message);
        public Task DispatchAsync<TData>(IMessage<TData> message);
        public Task DispatchAsync<TData>(ICommand<TData> command);
    }
}
