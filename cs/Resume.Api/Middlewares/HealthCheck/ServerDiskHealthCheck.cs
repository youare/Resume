using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace Resume.Api.Middlewares.HealthCheck
{
    public class ServerDiskHealthCheck : IHealthCheck
    {
        private readonly IConfigManager _configManager;

        public ServerDiskHealthCheck(IConfigManager configManager)
        {
            _configManager = configManager;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var data = IsUnix ? GetUnix() : GetWindows();
            var status = HealthStatus.Healthy;
            foreach (var item in data)
            {
                if (item.Usage > _configManager.AppConfig.HealthCheckConfig.ServerDisk.DegradeThreshold) status = HealthStatus.Degraded;
                if (item.Usage > _configManager.AppConfig.HealthCheckConfig.ServerDisk.UnhealthyThreshold)
                {
                    status = HealthStatus.Unhealthy;
                    break;
                }
            }
            var dic = new Dictionary<string, object>
            {
                {"Data", data }
            };
            var result = new HealthCheckResult(status, null, null, dic);
            return await Task.FromResult(result);
        }
        private bool IsUnix => RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        private IList<Disk> GetWindows()
        {
            var info = new ProcessStartInfo
            {
                FileName = "wmic",
                Arguments = "logicaldisk get freespace, name, size",
                RedirectStandardOutput = true
            };
            using var process = Process.Start(info);
            var output = process.StandardOutput.ReadToEnd();
            var lines = output.Split("\n").Select(x=>x.Trim()).Where(x=>!string.IsNullOrEmpty(x)).Skip(1).ToList();
            var list = new List<Disk>();

            foreach(var line in lines)
            {
                var array = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                var name = array[1];
                if (!string.IsNullOrEmpty(_configManager.AppConfig.HealthCheckConfig.ServerDisk.IgnoreCommaSeperated) && 
                    _configManager.AppConfig.HealthCheckConfig.ServerDisk.IgnoreCommaSeperated.Contains(name)) continue;
                var total = decimal.Parse(array[2]) / 1024 / 1024 / 1024;
                var free = decimal.Parse(array[0]) / 1024 / 1024 / 1024;
                var used = total - free;
                var data = new Disk(name, free, used, total);
                list.Add(data);
            }
            return list;
        }
        private IList<Disk> GetUnix()
        {
            var info = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = "-c \"df --total\"",
                RedirectStandardOutput = true
            };
            using var process = Process.Start(info);
            var output = process.StandardOutput.ReadToEnd();
            Console.WriteLine(output);

            var lines = output.Split("\n").Select(x=>x.Trim()).Skip(1).ToList();
            var memory = lines[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);
            var list = new List<Disk>();
            foreach(var line in lines)
            {
                var array = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                if(array.Length >= 4)
                {
                    var name = array[0];
                    if (_configManager.AppConfig.HealthCheckConfig.ServerDisk.IgnoreCommaSeperated.Contains(name)) continue;
                    var total = decimal.Parse(array[1]) / 1024 / 1024;
                    var used = decimal.Parse(array[2]) / 1024 / 1024;
                    var free = decimal.Parse(array[3]) / 1024 / 1024;
                    var data = new Disk(name, free, used, total);
                    list.Add(data);

                }
            }
            return list;
        }
    }

    public class Disk
    {
        public string Name { get;  }
        public decimal Free { get;  }
        public decimal Used { get;  }
        public decimal Total { get;  }
        public decimal Usage => Used / Total;
        public Disk(string name, decimal free, decimal used, decimal total)
        {
            Name = name;
            Free = free;
            Used = used;
            Total = total;
        }
    }
}
