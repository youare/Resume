using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Guardian
{
    public class ServiceInstanceServiceModel
    {
        public string Service { get; set; }
        public string InstanceId { get; set; }
        public string Scheme { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string HealthCheckStatus { get; set; }
        public string Address { get
            {
                var portString = Port == 0 || Port == 80 || Port == 443 ? "" : $":{Port}";
                if (string.IsNullOrEmpty(Scheme)) return $"{Host}{portString}";
                return $"{Scheme}://{Host}{portString}";
            } }
    }
}
