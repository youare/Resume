using Client.Guardian;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Resume.Api.Middlewares.HealthCheck
{
    public class HealthCheckPublisher : IHealthCheckPublisher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfigManager _configManager;

        public HealthCheckPublisher(IServiceProvider serviceProvider, IConfigManager configManager)
        {
            _serviceProvider = serviceProvider;
            _configManager = configManager;
        }

        public async Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var guardianClient = scope.ServiceProvider.GetService<IGuardianClient>();
            var dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(report));
            dic["Service"] = _configManager.AppConfig.ApplicationName;
            dic["InstanceId"] = _configManager.AppConfig.InstanceId;
            dic["DbHost"] = _configManager.AppConfig.ConnectionString.Split(";").Select(x => x.ToLower()).FirstOrDefault(x => x.Contains("host"))?.Replace("host=", "") ?? string.Empty;
            dic["MachineName"] = Environment.MachineName;
            dic["Scheme"] = _configManager.AppConfig.UriConfig.Self.Scheme;
            dic["Host"] = _configManager.AppConfig.UriConfig.Self.Host;
            dic["Port"] = _configManager.AppConfig.UriConfig.Self.Port;
            dic["HealthCheckToleranceInSeconds"] = _configManager.AppConfig.HealthCheckConfig.TolenranceInSeconds;
            //await guardianClient.PostHealthCheckAsync(dic, string.Empty);
            
        }
    }
}
