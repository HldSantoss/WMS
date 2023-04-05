using System;
using System.Text;
using System.Text.Json;
using Domain.Entities;
using Domain.Entities.Intelipost;
using Infra.Intelipost.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;
using static System.Net.Mime.MediaTypeNames;

namespace Infra.Intelipost.Operations
{
    public class ApiIntelipost : IApiIntelipost
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AsyncCircuitBreakerPolicy _circuitBreaker;
        private readonly ILogger<ApiIntelipost> _logger;

        public ApiIntelipost(IConfiguration configuration, IHttpClientFactory httpClientFactory, AsyncCircuitBreakerPolicy circuitBreaker, ILogger<ApiIntelipost> logger)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _circuitBreaker = circuitBreaker;
            _logger = logger;
        }

        public async Task<ReturnOrder?> CreateOrderonIntelipost(OrderIntelipost order)
        {
            var client = _httpClientFactory.CreateClient("Intelipost");
            var apiKey = _configuration.GetSection("Api:Intelipost:Api-Key").Value;
            var payload = new StringContent(JsonSerializer.Serialize(order), Encoding.UTF8, Application.Json);

            client.DefaultRequestHeaders.Add("api-key", apiKey);
            var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
            {
                return client.PostAsync($"/api/v1/shipment_order", payload);
            });

            var json = response.Content.ReadAsStringAsync().Result ?? throw new ArgumentNullException("body service layer");
            _logger.LogDebug($"IIntelipostService status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");

            return JsonSerializer.Deserialize<ReturnOrder>(json);
        }

        public async Task ReadyForShipmentOrderOnIntelipost(string numAtCard)
        {
            var client = _httpClientFactory.CreateClient("Intelipost");
            var apiKey = _configuration.GetSection("Api:Intelipost:Api-Key").Value;

            client.DefaultRequestHeaders.Add("api-key", apiKey);
            var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
            {
                return client.PostAsync("/api/v1/shipment_order/ready_for_shipment",
                            new StringContent(JsonSerializer.Serialize(new { order_number = numAtCard}), Encoding.UTF8, Application.Json));
            });

            _logger.LogDebug($"IIntelipostService status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");
        }

        public async Task SetTrackingOrderOnIntelipost(TrackingData order)
        {
            var client = _httpClientFactory.CreateClient("Intelipost");
            var apiKey = _configuration.GetSection("Api:Intelipost:Api-Key").Value;

            client.DefaultRequestHeaders.Add("api-key", apiKey);
            var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
            {
                return client.PostAsync("/api/v1/shipment_order/set_tracking_data",
                            new StringContent(JsonSerializer.Serialize(order), Encoding.UTF8, Application.Json));
            });

            _logger.LogDebug($"SetTrackingOrderOnIntelipost status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");
        }

        public async Task ShippedOrderOnIntelipost(OrderIntelipost order)
        {
            var client = _httpClientFactory.CreateClient("Intelipost");
            var apiKey = _configuration.GetSection("Api:Intelipost:Api-Key").Value;

            client.DefaultRequestHeaders.Add("api-key", apiKey);
            var response = await _circuitBreaker.ExecuteAsync<HttpResponseMessage>(() =>
            {
                return client.PostAsync("/api/v1/shipment_order/shipped",
                            new StringContent(JsonSerializer.Serialize(order), Encoding.UTF8, Application.Json));
            });

            _logger.LogDebug($"IIntelipostService status={response.StatusCode} - body={response.Content.ReadAsStringAsync().Result}");
        }
    }
}

