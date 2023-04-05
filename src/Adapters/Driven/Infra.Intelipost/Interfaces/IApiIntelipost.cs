using System;
using Domain.Entities.Intelipost;

namespace Infra.Intelipost.Interfaces
{
    public interface IApiIntelipost
    {
        Task<ReturnOrder?> CreateOrderonIntelipost(OrderIntelipost order);
        Task ReadyForShipmentOrderOnIntelipost(string numAtCard);
        Task ShippedOrderOnIntelipost(OrderIntelipost order);
        Task SetTrackingOrderOnIntelipost(TrackingData order);
    }
}

