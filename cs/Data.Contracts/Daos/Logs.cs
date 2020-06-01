using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Contracts.Daos
{
    public class Logs:BaseDao<Guid>
    {
        public DateTime? Date { get; private set; }
        public string CorrelationId { get; private set; }
        public string ApplicationName { get; private set; }
        public string Username { get; private set; }
        public string Level { get; private set; }
        public string RequestId { get; private set; }
        public string ThreadId { get; private set; }
        public string SourceContext { get; private set; }
        public string ActionParams { get; private set; }
        public int? ElapsedMilliseconds { get; private set; }
        public string Message { get; private set; }
        public string MessageTemplate { get; private set; }
        public DateTime? AuditTime { get; private set; }
        public string Exception { get; private set; }
        public string MachineName { get; private set; }
        public string Properties { get; private set; }
        public string PropTest { get; private set; }
        private Logs() { }
        public Logs(DateTime? date, string correlationId, string applicationName, string username, string level, string requestId, string threadId, string sourceContext, string actionParams, int? elapsedMilliseconds, string message, string messageTemplate, DateTime? auditTime, string exception, string machineName, string properties, string propTest)
        {
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
    }
}
