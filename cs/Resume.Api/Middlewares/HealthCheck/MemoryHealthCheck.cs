using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Resume.Api.Middlewares.HealthCheck
{
    public class MemoryHealthCheck:IHealthCheck
    {
        private readonly IConfigManager _configManager;

        public MemoryHealthCheck(IConfigManager configManager)
        {
            _configManager = configManager;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var proc = Process.GetCurrentProcess();
            var memoryInMB = proc.WorkingSet64 / 1024 / 1024;
            var status = HealthStatus.Healthy;
            if(memoryInMB > _configManager.AppConfig.HealthCheckConfig.Memory.DegradeThreshold)
            {
                status = HealthStatus.Degraded;
            }
            if (memoryInMB > _configManager.AppConfig.HealthCheckConfig.Memory.UnhealthyThreshold)
            {
                status = HealthStatus.Unhealthy;
            }
            var data = new Dictionary<string, object> { };
            var result = new HealthCheckResult(status, $"Current memory usage is {(decimal)memoryInMB / 1014}GB", null, data);
            return await Task.FromResult(result);
        }
    }
}
