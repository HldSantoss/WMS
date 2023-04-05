using System;
using System.Net;
using System.Text.Json;
using Domain.Entities;
using Infra.ServiceLayer.Interfaces;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;

namespace Infra.ServiceLayer.Operations
{
	public class ActivityLogSLService : IActivityLogSLService
	{
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AsyncCircuitBreakerPolicy _circuitBreaker;
        private readonly ILoginSLService _loginService;
        private readonly ILogger<GoodsReceivingSLService> _logger;

        public ActivityLogSLService(IConfiguration configuration,
                                    IHttpClientFactory httpClientFactory,
                                    AsyncCircuitBreakerPolicy circuitBreaker,
                                    ILoginSLService loginService,
                                    ILogger<GoodsReceivingSLService> logger)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _circuitBreaker = circuitBreaker;
            _loginService = loginService;
            _logger = logger;
        }

        public async Task CreateActivityLogAsync(ActivityLog activityLog, int tryLogin = 0)
        {
            var client = _httpClientFactory.CreateClient("ServiceLayer");
            client.DefaultRequestHeaders.Add("Prefer", "return-no-content");
            var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
            {
                return client.PostAsync($"/b1s/v1/ACTIVITYLOGS", new StringContent(JsonSerializer.Serialize(activityLog)));
            });

            if (response.StatusCode == HttpStatusCode.Unauthorized && tryLogin == 0)
            {
                await _loginService.LoginAsync();
                await CreateActivityLogAsync(activityLog, 1);
                return;
            }

            if (response.StatusCode != HttpStatusCode.NoContent)
                throw new Exception($"CreateActivityLogAsync - status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            _logger.LogDebug($"CreateActivityLogAsync status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

        }
    }
}

