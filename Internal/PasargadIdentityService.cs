using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ParbadGateway.PasargadRest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ParbadGateway.PasargadRest.Internal
{
    internal class PasargadIdentityService : IPasargadIdentityService
    {
        private const string AccessCacheKey = "PasargadRestAccessCacheKey";

        private readonly IMemoryCache _cache;
        private readonly HttpClient httpClient;
        private readonly ILogger<PasargadIdentityService> logger;
        private readonly PasargadRestGatewayOptions gatewayOptions;

        public PasargadIdentityService(IMemoryCache cache, IHttpClientFactory httpClient, ILogger<PasargadIdentityService> logger, IOptions<PasargadRestGatewayOptions> options            )
        {

            _cache = cache;
            this.httpClient = httpClient.CreateClient();
            this.logger = logger;
            gatewayOptions = options.Value;
        }

        public async Task<string> GetAccessTokenAsync(PasargadRestGatewayAccount account)
        {
            if (_cache.TryGetValue(AccessCacheKey + account.Name, out string accessToken))
                return accessToken;
            var loginResponse = await GetAccessTokenInternalAsync(account);
            if (loginResponse.resultCode == 0 )
            {
                _cache.Set(AccessCacheKey + account.Name , "Bearer " +  loginResponse.token, DateTimeOffset.Now.AddMinutes(5));
                return "Bearer " + loginResponse.token;
            }
            throw new Exception("login error: " + loginResponse.resultMsg + ", Error Code:" + loginResponse.resultCode);
        }


        private async Task<PasargadRestTokenResponse> GetAccessTokenInternalAsync(PasargadRestGatewayAccount account)
        {

            var request = new PasargadRestTokenRequest()
            {
               password = account.Password , username = account.UserName
            };
            return await LoginAsync(request);
        }

        private async Task<PasargadRestTokenResponse> LoginAsync(PasargadRestTokenRequest loginRequest)
        {

            var jsonRequest = JsonConvert.SerializeObject(loginRequest);
            var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            try
            {
                httpClient.BaseAddress = new Uri(gatewayOptions.BaseEndpoint);
                var response = await httpClient.PostAsync(gatewayOptions.GetTokenEndpoint, httpContent);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var loginResponse = JsonConvert.DeserializeObject<PasargadRestTokenResponse>(jsonResponse);
                    return loginResponse;
                }
                else
                {
                    switch ((int)response.StatusCode)
                    {
                        case 401:
                            throw new Exception("Unauthorized - کاربر نامعتبر است.");
                        case 403:
                            throw new Exception("Forbidden - دسترسی به سرویس نا معتبر است.");
                        case 500:
                            throw new Exception("Internal Server Error - رخ داد خطا در سرور.");
                        default:
                            response.EnsureSuccessStatusCode();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogCritical($"An error occurred during login: {ex.Message}", ex);
                throw;
            }
            return null;
        }

    }
}
