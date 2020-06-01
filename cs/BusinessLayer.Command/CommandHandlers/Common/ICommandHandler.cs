using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Command.CommandHandlers
{
    public interface IMessage
    {
        string Type { get; }
        string CorrelationId { get; }
        DateTime AuditTime { get; }
    }
    public interface IMessage<TData> : IMessage
    {
        TData Data { get; }
    }
    public interface IEvent : IMessage
    {

    }
    public interface IEvent<TData> : IMessage<TData>
    {

    }
    public interface ICommand : IMessage
    {

    }
    public interface ICommand<TData> : ICommand, IMessage<TData>
    {

    }
    public interface ICommandHandler<TData, TCommand>
        where TCommand : ICommand<TData>
    {
        Task HandleAsync(TCommand command); 
    }
}
