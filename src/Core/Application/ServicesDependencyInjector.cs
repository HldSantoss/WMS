using Application.Repositories;
using Application.Services;
using Application.Services.Orders;
using Domain.Repositories;
using Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class ServicesDependencyInjector
    {
        public static void AddServicesInjector(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IGoodsReceivingService, GoodsReceivingService>();
            services.AddScoped<ITransferService, TransferService>();
            services.AddScoped<IPickingService, PickingService>();
            services.AddScoped<ICheckoutService, CheckoutService>();
            services.AddScoped<IPackingListService, PackingListService>();
            services.AddScoped<IUserGroupService, UserGroupService>();
            services.AddScoped<IOrdersService, OrdersService>();
            services.AddScoped<ITagCorreiosService, TagCorreiosService>();

            services.AddScoped<IActivityRepository, ActivityRepository>();
        }
    }
}