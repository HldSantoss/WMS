using System;
using Domain.Entities;

namespace Infra.ServiceLayer.Interfaces
{
	public interface ICheckoutSLService
	{
        Task<long?> GetInvoiceEntry(long orderEntry, int tryLogin = 0);
        Task<Picking?> GetInvoiceAsync(long invoiceEntry, int tryLogin = 0);
        Task<(string?, string?)> GetLabel(long orderEntry, int tryLogin = 0);
        Task UpdateCheckoutStatusAsync(string status, long docEntry, string? sro = null, int tryLogin = 0);
    }
}

