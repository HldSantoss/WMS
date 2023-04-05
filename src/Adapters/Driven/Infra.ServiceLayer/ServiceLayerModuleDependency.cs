using System.Net;
using Infra.ServiceLayer.Interfaces;
using Infra.ServiceLayer.Operations;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace src.Adapters.Driven.Infra.ServiceLayer;

public static class ServiceLayerModuleDependency
{
    public static void AddServiceLayerModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ILoginSLService, LoginSLService>();
        services.AddSingleton<IInventorySLService, InventorySLService>();
        services.AddSingleton<IGoodsReceivingSLService, GoodsReceivingSLService>();
        services.AddSingleton<IPickingSLService, PickingSLService>();
        services.AddSingleton<IPackingListSLService, PackingListSLService>();
        services.AddSingleton<IGroupListingSLService, GroupListingSLService>();
        services.AddSingleton<ICheckoutSLService, CheckoutSLService>();
        services.AddSingleton<IActivityLogSLService, ActivityLogSLService>();
        services.AddSingleton<IOrdersSLService, OrdersSLService>();
        services.AddSingleton<IBranchesSLService, BranchesSLService>();
        services.AddSingleton<ITagCorreiosSLService, TagCorreiosSLService>();

        services.AddSingleton(CircuitBreaker.CreatePolicy());
        services.AddHttpClient("ServiceLayer", client =>
        {
            client.BaseAddress = new Uri(configuration.GetSection("Api:ServiceLayer:Uri").Value);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
        })
        .ConfigureHttpMessageHandlerBuilder(builder =>
        {
            builder.PrimaryHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (m, c, ch, e) => true
            };
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

    private static readonly HttpStatusCode[] httpStatusCodesWorthRetrying = new [] {
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