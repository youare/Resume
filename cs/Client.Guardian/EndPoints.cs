using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Guardian
{
    public static class EndPoints
    {
        public static string PublishHealthCheck => $"api/v1/healthcheck";
        public static string GetServiceInstance => $"api/v1/serviceInstance";
    }
}
