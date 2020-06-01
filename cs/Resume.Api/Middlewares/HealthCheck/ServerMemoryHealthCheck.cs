using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Resume.Api.Middlewares.HealthCheck
{
    public class ServerMemoryHealthCheck : IHealthCheck
    {
        private readonly IConfigManager _configManager;

        public ServerMemoryHealthCheck(IConfigManager configManager)
        {
            _configManager = configManager;
        }
        private bool IsUnix => RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var data = IsUnix ? GetUnix() : GetWindows();
            var percentUsed = 100 * data.Used / data.Total;
            var status = HealthStatus.Healthy;
            if(percentUsed > _configManager.AppConfig.HealthCheckConfig.ServerMemory.DegradeThreshold)
            {
                status = HealthStatus.Degraded;
            }
            if(percentUsed > _configManager.AppConfig.HealthCheckConfig.ServerMemory.UnhealthyThreshold)
            {
                status = HealthStatus.Unhealthy;
            }
            var dic = new Dictionary<string, object>
            {
                {"Data", data }
            };
            var result = new HealthCheckResult(status, null, null, dic);
            return await Task.FromResult(result);
        }
        private Memory GetWindows()
        {
            var info = new ProcessStartInfo {
                FileName = "wmic",
                Arguments = "OS get FreePhysicalMemory,TotalVisiableMemorySize /Value",
                RedirectStandardOutput = true
            };
            using var process = Process.Start(info);
            var output = process.StandardOutput.ReadToEnd();
            var lines = output.Trim().Split("\n");
            var freeMemoryParts = lines[0].Split("=", StringSplitOptions.RemoveEmptyEntries);
            var totalMemoryParts = lines[1].Split("=", StringSplitOptions.RemoveEmptyEntries);
            var data = new Memory(Math.Round(decimal.Parse(totalMemoryParts[1]) / 1024 / 1024, 0), Math.Round(decimal.Parse(freeMemoryParts[0]) / 1024 / 1024, 0));
            return data;
        }
        private Memory GetUnix()
        {
            var info = new ProcessStartInfo { 
                FileName = "/bin/bash",
                Arguments = "-c \"free -m\"",
                RedirectStandardOutput = true
            };
            using var process = Process.Start(info);
            var output = process.StandardOutput.ReadToEnd();
            Console.WriteLine(output);
            var lines = output.Split("\n");
            var memory = lines[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);
            var data = new Memory(decimal.Parse(memory[1])/1024, decimal.Parse(memory[2])/1024);
            return data;
        }
    }

    public class Memory
    {
        public decimal Total { get; }
        public decimal Used { get; }
        public decimal Free => Total - Used;

        public Memory(decimal total, decimal used)
        {
            Total = total;
            Used = used;
        }
    }
}
