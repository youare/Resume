using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logging.Logging
{
    public static class DynamicLoggerconfigurationExtensions
    {
        public static LoggerConfiguration WithDynamicProperty(
            this LoggerEnrichmentConfiguration enrichmentConfiguration,
            string name,
            Func<object> valueProvider,
            bool destructureObjects = false,
            bool enrichWhenNullOrEmptyString = false,
            LogEventLevel minimumLevel = LogEventLevel.Verbose
            )
        {
            if (enrichmentConfiguration == null) throw new ArgumentNullException(nameof(enrichmentConfiguration));
            return enrichmentConfiguration.With(new DynamicEnricher(name, valueProvider, destructureObjects, enrichWhenNullOrEmptyString, minimumLevel));
        }
    }
    public class DynamicEnricher : ILogEventEnricher
    {
        private readonly string _propertyName;
        private readonly bool _enrichWhenNullOrEmptyString;
        private readonly LogEventLevel _minimumLevel;
        private readonly bool _descructureObjects;

        public Func<object> ValueProvider { get; set; }

        public DynamicEnricher(
            string propertyName,
            Func<object> valueProvider,
            bool descructureObjects = false,
            bool enrichWhenNullOrEmptyString = true,
            LogEventLevel minimumLevel = LogEventLevel.Verbose
        ) : base()
        {
            _propertyName = propertyName;
            _enrichWhenNullOrEmptyString = enrichWhenNullOrEmptyString;
            _minimumLevel = minimumLevel;
            _descructureObjects = descructureObjects;
            ValueProvider = valueProvider;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var logValue = ValueProvider();
            if ((_enrichWhenNullOrEmptyString || (logValue != null || (logValue is String && !String.IsNullOrEmpty((string)logValue)))) && logEvent.Level > _minimumLevel)
            {
                var prop = propertyFactory.CreateProperty(_propertyName, logValue, _descructureObjects);
                logEvent.AddPropertyIfAbsent(prop);
            }
        }
    }
}
