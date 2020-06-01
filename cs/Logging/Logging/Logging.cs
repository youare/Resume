using NpgsqlTypes;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.PostgreSQL;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Logging.Logging
{
    public static class Logging
    {
        public static void ConfigureLogging(string applicationName, string instanceId, string connectionString, IList<(string name, LogEventLevel level)> logConfigs)
        {
            Serilog.Debugging.SelfLog.Enable(err => Console.WriteLine(err));
            var name = Assembly.GetExecutingAssembly().GetName();
            var columnWriters = new Dictionary<string, ColumnWriterBase> {
                { "id", new SinglePropertyColumnWriter("id", PropertyWriteMethod.Raw, NpgsqlDbType.Uuid)},
                { "date", new SinglePropertyColumnWriter("date", PropertyWriteMethod.Raw, NpgsqlDbType.Date)},
                { "application_name", new SinglePropertyColumnWriter("ApplicationName")},
                { "username", new SinglePropertyColumnWriter("username")},
                { "correlation_id", new SinglePropertyColumnWriter("CorrelationId")},
                { "level", new LevelColumnWriter(true, NpgsqlDbType.Varchar)},
                { "request_id", new SinglePropertyColumnWriter("RequestId")},
                { "thread_id", new SinglePropertyColumnWriter("ThreadId")},
                { "source_context", new SinglePropertyColumnWriter("SourceContext")},
                { "action_params", new SinglePropertyColumnWriter("action_params")},
                { "elapsed_milliseconds", new SinglePropertyColumnWriter("ellapsed_milliseconds", PropertyWriteMethod.Raw, NpgsqlDbType.Integer)},
                { "message", new RenderedMessageColumnWriter(NpgsqlDbType.Text)},
                { "message_template", new RenderedMessageColumnWriter(NpgsqlDbType.Text)},
                { "exception", new ExceptionColumnWriter(NpgsqlDbType.Text)},
                { "machine_name", new SinglePropertyColumnWriter("MachineName", PropertyWriteMethod.ToString, NpgsqlDbType.Text, "l")},
                { "audit_time", new TimestampColumnWriter(NpgsqlDbType.Timestamp)},
                { "properties", new LogEventSerializedColumnWriter(NpgsqlDbType.Text)},
                { "props_text", new PropertiesColumnWriter(NpgsqlDbType.Text)},
            };

            var config = new LoggerConfiguration();
            foreach(var item in logConfigs)
            {
                if(item.name == "Default")
                {
                    SetLogLevel(config.MinimumLevel, item.level);
                }
                else
                {
                    config.MinimumLevel.Override(item.name, item.level);
                }
            }
            config
                .Enrich.WithExceptionDetails()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .Enrich.WithCorrelationId()
                .Enrich.WithProperty("assembly", $"{name.Name}")
                .Enrich.WithDynamicProperty("id", ()=>Guid.NewGuid())
                .Enrich.WithProperty("version", $"{name.Version}")
                .Enrich.WithDynamicProperty("date", () => DateTime.Now.Date)
                .Enrich.WithProperty("ApplicationName", applicationName)
                .Enrich.WithProperty("InstanceId", instanceId)
                .WriteTo.Console()
                .WriteTo.PostgreSQL(connectionString, "logs", columnWriters);
            Log.Logger = config.CreateLogger();
            AppDomain.CurrentDomain.ProcessExit += (s, e) => Log.CloseAndFlush();
        }
        private static LoggerConfiguration SetLogLevel(LoggerMinimumLevelConfiguration config, LogEventLevel level)
        {
            switch (level)
            {
                case LogEventLevel.Verbose: return config.Verbose();
                case LogEventLevel.Debug: return config.Debug();
                case LogEventLevel.Information: return config.Information();
                case LogEventLevel.Warning: return config.Warning();
                case LogEventLevel.Error: return config.Error();
                case LogEventLevel.Fatal: return config.Fatal();
                default: return config.Verbose();
            }
        }
    }
}
