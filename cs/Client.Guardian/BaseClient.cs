using IdentityModel.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Client.Guardian
{
    public abstract class BaseClient
    {
        protected abstract string Guardian { get; }
        protected abstract string Authority { get; }
        protected abstract string Secret { get; }
        protected abstract string ApplicationName { get; }
        protected abstract string Scope { get; }
        protected abstract IHttpClientFactory HttpClientFactory { get; }
        protected async Task<TokenResponse> GetTokenResponseAsync(string userToken)
        {
            using var authClient = HttpClientFactory.CreateClient();
            var discoveryDocument = await authClient.GetDiscoveryDocumentAsync(Authority);
            if (string.IsNullOrEmpty(userToken))
            {
                var tokenResponse = await authClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = discoveryDocument.TokenEndpoint,
                    ClientId = ApplicationName,
                    ClientSecret = Secret,
                    Scope = Scope
                });
                return tokenResponse;
            }
            else
            {
                var tokenOptions = new TokenClientOptions
                {
                    ClientId = ApplicationName,
                    ClientSecret = Secret,
                    Address = discoveryDocument.TokenEndpoint
                };
                var client = new TokenClient(authClient, tokenOptions);
                var dic = new Dictionary<string, string>
                {
                    {"scope", Scope },
                    {"token", userToken }
                };
                var tokenResponse = await client.RequestTokenAsync("delegation", dic);
                return tokenResponse;
            }
        }

        protected async Task<TServiceModel> GetResponseAsync<TServiceModel>(string url, string userToken)
        {
            using var httpClient = HttpClientFactory.CreateClient();
            var tokenResponse = await GetTokenResponseAsync(userToken);
            httpClient.SetBearerToken(tokenResponse.AccessToken);
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            using var result = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            await using var contentStream = await result.Content.ReadAsStreamAsync();
            var data = await System.Text.Json.JsonSerializer.DeserializeAsync<TServiceModel>(contentStream, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            return data;
        }

        protected async Task PostAsync<TData>(string url, TData data, string userToken)
        {
            using var httpClient = HttpClientFactory.CreateClient();
            var tokenResponse = await GetTokenResponseAsync(userToken);
            httpClient.SetBearerToken(tokenResponse.AccessToken);
            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            var jsonString = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            await httpClient.PostAsync(url, content);
        }
        public async Task<IList<ServiceInstanceServiceModel>> GetAvailableServicesAsync(string serviceName, string userToken)
        {
            using var httpClient = HttpClientFactory.CreateClient();
            var tokenResponse = await GetTokenResponseAsync(userToken);
            httpClient.SetBearerToken(tokenResponse.AccessToken);
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{Guardian}/{EndPoints.GetServiceInstance}?serviceName={serviceName}");
            using var result = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            await using var contentStream = await result.Content.ReadAsStreamAsync();
            var data = await System.Text.Json.JsonSerializer.DeserializeAsync<List<ServiceInstanceServiceModel>>(contentStream, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            return data;
        }
        public async Task<Dictionary<string, object>> GetHealthAsync(string url, string userToken)
        {
            return await GetResponseAsync<Dictionary<string, object>>(url, userToken);
        }
    }
}
