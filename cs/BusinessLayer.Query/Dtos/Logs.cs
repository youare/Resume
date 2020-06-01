using System;
using System.Collections.Generic;
using System.Text;
using Data.Contracts.Daos;
namespace BusinessLayer.Query.Dtos.Logs
{
    public class LogsReadDto
    {
        public Guid Id { get; }
        public DateTime? Date { get; }
        public string CorrelationId { get; }
        public string ApplicationName { get; }
        public string Username { get; }
        public string Level { get; }
        public string RequestId { get; }
        public string ThreadId { get; }
        public string SourceContext { get; }
        public string ActionParams { get; }
        public int? ElapsedMilliseconds { get; }
        public string Message { get; }
        public string MessageTemplate { get; }
        public DateTime? AuditTime { get; }
        public string Exception { get; }
        public string MachineName { get; }
        public string Properties { get; }
        public string PropTest { get; }

        public LogsReadDto(Guid id, DateTime? date, string correlationId, string applicationName, string username, string level, string requestId, string threadId, string sourceContext, string actionParams, int? elapsedMilliseconds, string message, string messageTemplate, DateTime? auditTime, string exception, string machineName, string properties, string propTest)
        {
            Id = id;
            Date = date;
            CorrelationId = correlationId;
            ApplicationName = applicationName;
            Username = username;
            Level = level;
            RequestId = requestId;
            ThreadId = threadId;
            SourceContext = sourceContext;
            ActionParams = actionParams;
            ElapsedMilliseconds = elapsedMilliseconds;
            Message = message;
            MessageTemplate = messageTemplate;
            AuditTime = auditTime;
            Exception = exception;
            MachineName = machineName;
            Properties = properties;
            PropTest = propTest;
        }

        public static LogsReadDto MapFrom(Data.Contracts.Daos.Logs item)
        {
            return new LogsReadDto(
                item.Id,
                item.Date,
                item.CorrelationId,
                item.ApplicationName,
                item.Username,
                item.Level,
                item.RequestId,
                item.ThreadId,
                item.SourceContext,
                item.ActionParams,
                item.ElapsedMilliseconds,
                item.Message,
                item.MessageTemplate,
                item.AuditTime,
                item.Exception,
                item.MachineName,
                item.Properties,
                item.PropTest
                );
        }
    }
}
