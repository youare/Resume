using Common;
using Common.Extensions;
using Logging.Logging;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Resume.Api.Middlewares
{
    public static class LoggingMiddleware
    {
        public static IServiceCollection ConfigureLogging(this IServiceCollection services)
        {
            var configManager = services.BuildServiceProvider().GetService<IConfigManager>();
            Logging.Logging.Logging.ConfigureLogging(
                configManager.AppConfig.ApplicationName,
                configManager.AppConfig.InstanceId, 
                configManager.AppConfig.ConnectionString, 
                configManager.AppConfig.LogConfig.LogEntryList.Select(x=> (x.Source, x.MinimumLevel.ToEnum<LogEventLevel>().Value)).ToList());
            return services.AddLogging(x => x.AddSerilog()).AddSingleton<AuditLog>().AddHttpContextAccessor();
        }
    }

    public class AuditLog : IActionFilter
    {
        private readonly ILoggerFactory _loggerFactory;
        private PerformanceTracker _tracker;

        public AuditLog(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!(context.ActionDescriptor is ControllerActionDescriptor cad)) return;
            var controllerAction = $"{cad.ControllerTypeInfo}.{cad.ActionName}";
            var logger = _loggerFactory.CreateLogger($"{controllerAction}");
            if (context.ActionArguments?.Count > 0)
            {
                var dic = new Dictionary<object, object>();
                foreach(var key in context.ActionArguments.Keys)
                {
                    dic[key] = context.ActionArguments[key];
                }
                _tracker = new PerformanceTracker(controllerAction, logger, Optional<IReadOnlyDictionary<object, object>>.Some(dic));
            }
            else
            {
                _tracker = new PerformanceTracker(controllerAction, logger, Optional<IReadOnlyDictionary<object, object>>.Empty);
            }
     
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            _tracker?.Stop();
        }
    }

    public class PerformanceTracker
    {
        private readonly string _whatsBeingTracked;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        private Optional<IReadOnlyDictionary<object, object>> _moreCustomInfo;
        private readonly Stopwatch _stopWatch;

        public PerformanceTracker(string whatsBeingTracked, Microsoft.Extensions.Logging.ILogger logger, Optional<IReadOnlyDictionary<object, object>> moreCustomInfo)
        {
            _whatsBeingTracked = whatsBeingTracked;
            _logger = logger;
            _moreCustomInfo = moreCustomInfo;
            _stopWatch = new Stopwatch();
            _stopWatch.Start();
        }
        public void Stop()
        {
            if (_stopWatch is null) return;
            _stopWatch.Stop();
            if (_moreCustomInfo.HasValue)
                _logger.LogInformation(LogTemplate.ResumeApi_PerformanceTracker_Stop_PerformanceTracking_With_Params, _whatsBeingTracked, _stopWatch.ElapsedMilliseconds, _moreCustomInfo.Value);
            else
                _logger.LogInformation(LogTemplate.ResumeApi_PerformanceTracker_Stop_PerformanceTracking, _whatsBeingTracked, _stopWatch.ElapsedMilliseconds);
        }
    }
}
