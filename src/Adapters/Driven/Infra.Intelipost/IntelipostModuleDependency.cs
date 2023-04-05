using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Infra.Intelipost.Operations;
using Infra.Intelipost.Interfaces;

namespace Infra.Intelipost
{
	public static class IntelipostModuleDependency
	{
        public static void AddIntelipostServiceDependency(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IApiIntelipost, ApiIntelipost>();

            services.AddSingleton(CircuitBreaker.CreatePolicy());
            services.AddHttpClient("Intelipost", client =>
            {
                client.BaseAddress = new Uri(configuration.GetSection("Api:Intelipost:Uri").Value);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            })
            .SetHandlerLifetime(TimeSpan.FromMinutes(10))
            .AddPolicyHandler(RetryPolicy());
        }

        private static AsyncRetryPolicy<HttpResponseMessage> RetryPolicy()
        {
            return Policy.Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(msg =>
                    httpStatusCodesWorthRetrying.Contains(msg.StatusCode))
                .WaitAndRetryAsync(3, retryAttempt => {
                    Console.WriteLine($"Retrying in {retryAttempt} seconds get http client");
                    return TimeSpan.FromSeconds(10);
                });
        }

        private static readonly HttpStatusCode[] httpStatusCodesWorthRetrying = new[] {
        HttpStatusCode.BadGateway,
        HttpStatusCode.ServiceUnavailable,
        HttpStatusCode.GatewayTimeout
    };

        public static class CircuitBreaker
        {
            public static AsyncCircuitBreakerPolicy CreatePolicy()
            {
                return Policy
                    .Handle<HttpRequestException>()
                    .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30),
                        onBreak: (_, _) =>
                        {
                            LogCircuitState("Open (onBreak)");
                        },
                        onReset: () =>
                        {
                            LogCircuitState("Closed (onReset)");
                        },
                        onHalfOpen: () =>
                        {
                            LogCircuitState("Half Open (onHalfOpen)");
                        });
            }

            private static void LogCircuitState(string descStatus)
            {
                Console.Out.WriteLine($" ***** Estado do Circuito: {descStatus} **** ");
            }
        }
    }
}

