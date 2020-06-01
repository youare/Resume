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
    public class ServerCpuHealthCheck:IHealthCheck
    {
        private readonly IConfigManager _configManager;

        public ServerCpuHealthCheck(IConfigManager configManager)
        {
            _configManager = configManager;
        }

        private bool IsUnix => RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var data = IsUnix ? GetUnix() : GetWindows();
            var percentUsed = 100 * data.Used;
            var status = HealthStatus.Healthy;
            if (percentUsed > _configManager.AppConfig.HealthCheckConfig.ServerCpu.DegradeThreshold)
            {
                status = HealthStatus.Degraded;
            }
            if (percentUsed > _configManager.AppConfig.HealthCheckConfig.ServerCpu.UnhealthyThreshold)
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
        private Cpu GetWindows()
        {
            var info = new ProcessStartInfo
            {
                FileName = "wmic",
                Arguments = "cpu get loadpercentage",
                RedirectStandardOutput = true
            };
            using var process = Process.Start(info);
            var output = process.StandardOutput.ReadToEnd();

            var lines = output.Trim().Split("\n").Select(x=>x.Trim()).Where(x=> !string.IsNullOrEmpty(x)).ToList();

            var used = decimal.Parse(lines[1]) / 100;
            var total = 1m;
            var free = total - used;
            var data = new Cpu(used, free, total, null, null, null, null, null, null);

            return data;
        }
        private Cpu GetUnix()
        {
            var output = "";
            var info = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = "-c \"iostat\"",
                RedirectStandardOutput = true
            };
            using (var process = Process.Start(info))
            {
                output = process.StandardOutput.ReadToEnd();
                Console.WriteLine(output);
            }
            var lines = output.Split("\n").Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToList();
            var targetIndex = 0;
            foreach(var line in lines)
            {
                targetIndex++;
                if (line.Contains("avg-cpu")) break;
            }
            var cpus = lines[targetIndex].Split(" ", StringSplitOptions.RemoveEmptyEntries);
            var user = decimal.Parse(cpus[0]) / 100;
            var nice = decimal.Parse(cpus[1]) / 100;
            var system = decimal.Parse(cpus[2]) / 100;
            var iowait = decimal.Parse(cpus[3]) / 100;
            var steal = decimal.Parse(cpus[4]) / 100;
            var idle = decimal.Parse(cpus[5]) / 100;
            var used = user + nice + system + iowait + steal;
            var total = user + nice + system + iowait + steal + idle;
            var free = total - used;
            return new Cpu(used, free, total, user, nice, system, iowait, steal, idle);
        }
    }
    public class Cpu
    {
        public decimal Used { get; }
        public decimal Free { get; }
        public decimal Total { get; }
        public decimal? User { get; }
        public decimal? Nice { get; }
        public decimal? System { get; }
        public decimal? IOWait { get; }
        public decimal? Steal { get; }
        public decimal? Idle { get; }

        public Cpu(decimal used, decimal free, decimal total, decimal? user, decimal? nice, decimal? system, decimal? iOWait, decimal? steal, decimal? idle)
        {
            Used = used;
            Free = free;
            Total = total;
            User = user;
            Nice = nice;
            System = system;
            IOWait = iOWait;
            Steal = steal;
            Idle = idle;
        }
    }
}
