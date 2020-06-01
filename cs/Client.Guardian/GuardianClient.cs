using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Client.Guardian
{
    public interface IGuardianClient
    {
        Task PostHealthCheckAsync(Dictionary<string, object> healthReport, string userToken);
    }
    public class GuardianClient : BaseClient, IGuardianClient
    {
        protected override string Guardian { get; }

        protected override string Authority { get; }

        protected override string Secret { get; }

        protected override string ApplicationName { get; }

        protected override string Scope { get; }

        protected override IHttpClientFactory HttpClientFactory { get; }

        public GuardianClient(string guardian, string authority, string secret, string applicationName, IHttpClientFactory httpClientFactory)
        {
            Guardian = guardian;
            Authority = authority;
            Secret = secret;
            ApplicationName = applicationName;
            Scope = "GuardianApi";
            HttpClientFactory = httpClientFactory;
        }
        public async Task PostHealthCheckAsync(Dictionary<string, object> healthReport, string userToken)
        {
            await PostAsync($"{Guardian}/{EndPoints.PublishHealthCheck}", healthReport, userToken);
        }
    }
}
