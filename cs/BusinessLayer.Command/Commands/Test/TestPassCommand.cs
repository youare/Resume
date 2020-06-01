using System;
using BusinessLayer.Command.CommandHandlers;

namespace BusinessLayer.Command.Commands.Test
{
    public class TestPassCommand: ICommand<int>
    {

        public int Data { get; }

        public string Type { get; }

        public string CorrelationId { get; }

        public DateTime AuditTime { get; }

        public TestPassCommand(int data, string type, string correlationId, DateTime auditTime)
        {
            Data = data;
            Type = type ?? throw new ArgumentNullException(nameof(type));
            CorrelationId = correlationId ?? throw new ArgumentNullException(nameof(correlationId));
            AuditTime = auditTime;
        }
    }
    public class TestFailCommand : ICommand<int>
    {

        public int Data { get; }

        public string Type { get; }

        public string CorrelationId { get; }

        public DateTime AuditTime { get; }

        public TestFailCommand(int data, string type, string correlationId, DateTime auditTime)
        {
            Data = data;
            Type = type ?? throw new ArgumentNullException(nameof(type));
            CorrelationId = correlationId ?? throw new ArgumentNullException(nameof(correlationId));
            AuditTime = auditTime;
        }
    }
}
