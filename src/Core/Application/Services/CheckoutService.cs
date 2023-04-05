using Application.Repositories;
using Domain.Entities;
using Domain.Entities.Orders;
using Domain.Enums;
using Domain.Repositories;
using Domain.Services;
using Infra.ServiceLayer.Interfaces;
using Org.BouncyCastle.Asn1.X509;
using System.Globalization;

namespace Application.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly IPickingSLService _pickingSLService;
        private readonly ICheckoutSLService _checkoutSLService;
        private readonly IBranchesSLService _branchesSLService;
        private readonly ITagCorreiosService _tagCorreiosService;
        private readonly IActivityRepository _activityyRepository;

        public CheckoutService(IPickingSLService pickingSLService,
                               ICheckoutSLService checkoutSLService,
                               IBranchesSLService branchesSLService,
                               ITagCorreiosService tagCorreiosService,
                               IActivityRepository activityyRepository)
        {
            _pickingSLService = pickingSLService;
            _checkoutSLService = checkoutSLService;
            _branchesSLService = branchesSLService;
            _tagCorreiosService = tagCorreiosService;
            _activityyRepository = activityyRepository;
        }

        public async Task FinishAsync(long orderEntry, string userId = null)
        {
            await _pickingSLService.UpdatePickingStatusByCheckoutAsync(OrderStatusEnum.CanPacking.ToString(), orderEntry);
            var order = await _pickingSLService.GetPickingAsync(orderEntry);
         
            CultureInfo pt = new CultureInfo("pt-BR");
            Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR", false);
            var dateAjusted = DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss");
            await _activityyRepository.SaveActivity(new Activity(dateAjusted, userId, WmsAction.FinishCheckout.ToString(), orderEntry.ToString(), order.BPLId.ToString()));
        }

        public async Task<Picking?> GetOrderAsync(long orderEntry, string userId = null)
        {
            var invoiceEntry = await _checkoutSLService.GetInvoiceEntry(orderEntry);

            if (invoiceEntry == null)
                throw new Exception($"Nota fiscal do pedido {orderEntry} não existe");

            var invoice = await _checkoutSLService.GetInvoiceAsync((long)invoiceEntry);

            if (invoice == null)
                throw new Exception($"Nota fiscal do pedido {orderEntry} não existe");

            if (string.IsNullOrEmpty(invoice.AccessKey))
                throw new Exception($"Nota fiscal do pedido {orderEntry} não foi aprovada ainda, notifique o fiscal");

            var invoiceValid = invoice.Items.Where(p => p.TreeType != "iSalesTree").ToList() ?? throw new ArgumentNullException("Não possui itens validos para conferir");

            invoice.Items = invoiceValid;

            invoice.Carrier = invoice?.TaxExtension?.Carrier;
            if (!string.IsNullOrWhiteSpace(invoice?.TaxExtension?.Carrier))
                invoice.CarrierName = await _pickingSLService.GetCarrierName(invoice.TaxExtension.Carrier);

            await Labels(orderEntry, invoice);

            await _checkoutSLService.UpdateCheckoutStatusAsync(OrderStatusEnum.Checkingout.ToString(), orderEntry, invoice.TrackingCode);
           
            CultureInfo pt = new CultureInfo("pt-BR");
            Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR", false);
            var dateAjusted = DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss");
            await _activityyRepository.SaveActivity(new Activity(dateAjusted, userId, WmsAction.StartCheckout.ToString(), orderEntry.ToString(), invoice.BPLId.ToString()));

           return invoice;
        }

        private async Task Labels(long orderEntry, Picking invoice)
        {
           var (labelML, labelDanfe) = await _checkoutSLService.GetLabel(orderEntry);
            var labels = new List<Label>();

            if (labelML == null && invoice.Carrier == "F0000004")
            {
                throw new Exception("Pedido mercado livre sem etiqueta gerada, tente novamente em alguns minutos.");
            }

            if (labelDanfe == null && invoice.Carrier != "F0000004")
            {
                throw new Exception("Pedido ainda não possui etiqueta de Transportadora gerada (Não é Mercado Livre)");
            }

            if (!string.IsNullOrWhiteSpace(labelML))
                labels.Add(new Label() { Zpl = labelML });

            if (!string.IsNullOrWhiteSpace(labelDanfe))
                labels.Add(new Label() { Zpl = labelDanfe });

            invoice.Labels = labels;
        }
    }
}