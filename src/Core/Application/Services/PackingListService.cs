using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using DinkToPdf;
using DinkToPdf.Contracts;
using Domain.Adapters;
using Domain.Entities;
using Domain.Entities.Intelipost;
using Domain.Entities.Orders;
using Domain.Services;
using Infra.Intelipost.Interfaces;
using Infra.Pdf.Operations;
using Infra.ServiceLayer.Interfaces;
using Infra.ServiceLayer.Operations;
using Lextm.SharpSnmpLib;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Zebra.Sdk.Comm;

namespace Application.Services
{
    public class PackingListService : IPackingListService
    {
        private readonly IPackingListSLService _packingListSLService;
        private readonly IConverter _converter;
        private readonly IApiIntelipost _apiIntelipost;
        private readonly IOrdersSLService _ordersSLService;
        private readonly ILogger<PackingListService> _logger;
        private readonly IPublisher _publisher;
        private readonly IConfiguration _configuration;

        public PackingListService(IPackingListSLService packingListSLService, IConverter converter, IApiIntelipost apiIntelipost, IOrdersSLService ordersSLService, ILogger<PackingListService> logger, IPublisher publisher, IConfiguration configuration)
        {
            _packingListSLService = packingListSLService;
            _converter = converter;
            _apiIntelipost = apiIntelipost;
            _ordersSLService = ordersSLService;
            _logger = logger;
            _publisher = publisher;
            _configuration = configuration;
        }

        public async Task AddItemPackingListAsync(long packingListEntry, string keyNfe, bool resend)
        {
            var invoiceEntry = await _packingListSLService.GetInvoiceEntryAsync(keyNfe);

            if (invoiceEntry == default)
                throw new KeyNotFoundException($"Chave {keyNfe} não encontrada");

            var order = await _packingListSLService.GetInfoNFEAsync(keyNfe);

            if (order?.NF?.FirstOrDefault()?.CANCELED == "Y")
            {
                throw new Exception("Pedido cancelado, por favor devolva os itens ao estoque.");
            }

            if (order?.NF?.FirstOrDefault()?.U_CT_Qualidade == "C")
            {
                throw new Exception("Pedido bloqueado pelo SAC por favor notifique seu supervisor");
            }

            if(resend == false)
            {
                if (order?.NF?.FirstOrDefault()?.U_WMS_Status == "Shipped")
                {
                    throw new Exception("Pedido já enviado para transportadora.");
                }
            }

            var packingListItem = new List<PackingListUpsertItem>();
            packingListItem.Add(new PackingListUpsertItem(DateTime.Now.ToString("yyyy-MM-dd"), 
                                      DateTime.Now.ToString("HH:mm:ss"), 
                                      invoiceEntry.DocEntry, 
                                      keyNfe, 
                                      invoiceEntry.CardName, 
                                      order?.NF?.FirstOrDefault()?.DocEntry,
                                      invoiceEntry.SequenceSerial, 
                                      invoiceEntry.SeriesStr));

            var packingListUpsert = new PackingListUpsert(packingListItem);
            var orderSAP = await _ordersSLService.GetOrderToSendInIntelipost((long)order?.NF?.FirstOrDefault()?.DocEntry);

            await _packingListSLService.AddItemPackingList(packingListEntry, packingListUpsert);
            await _packingListSLService.UpdateOrderByPackingList(order.NF.FirstOrDefault().DocEntry, packingListEntry.ToString());

            var (obj, carrierName) = await BuildOrderIntelipost(orderSAP?.value?.FirstOrDefault(),resend);

            if (obj != null)
            {
                var trackingUrl = await _apiIntelipost.CreateOrderonIntelipost(obj);
                await _apiIntelipost.ReadyForShipmentOrderOnIntelipost(obj.Order_number);

                var objVTEX = new TrackingVtex
                {
                    trackingNumber = orderSAP?.value?.FirstOrDefault()?.U_CT_TrackingCode,
                    trackingUrl = trackingUrl?.content?.tracking_url.ToString(),
                    courier = carrierName,
                    dispatchedDate = DateTime.Now,
                    invoiceNumber = invoiceEntry.SequenceSerial.ToString(),
                    orderId = obj.Order_number
                 };

                JsonSerializerOptions options = new()
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                _publisher.Publish(JsonSerializer.Serialize(objVTEX, options),
                    $"storeId={orderSAP?.value?.FirstOrDefault().U_CT_Store}",
                   _configuration.GetSection("Rabbitmq:Shipped.Created:ExchangeName").Value);

            }
        }

        public async Task<string?> ClosePackingListAsync(long docEntry)
        {
            var updatePackList = await _packingListSLService.UpdateDateClosePackingListAsync(docEntry);

            if (updatePackList == null)
                return null;

            await _packingListSLService.ClosePackingListAsync(docEntry);

            return "ok";
        }

        public async Task RemoveItemPackingListAsync(long packingListEntry, string keyNfe)
        {
            var packingList = await _packingListSLService.GetPackingListAsync(packingListEntry);

            if (packingList == null)
                throw new ArgumentNullException($"Packing List {packingListEntry} não existe.");

            if (packingList.Status != "O")
                throw new ArgumentNullException($"Packing List {packingListEntry} está fechado e não pode ser alterado.");

            var item = packingList.Items?.Where(p => p.KeyNfe == keyNfe).FirstOrDefault();

            if (item == null || item == default)
                throw new ArgumentNullException($"Packing List {packingListEntry} não possui a chave de nota {keyNfe}.");

            packingList.Items?.Remove(item);

            await _packingListSLService.RemoveItemPackingListAsync(packingListEntry, packingList);
        }

        public async Task<string?> RemovePackingListAsync(long docEntry)
        {
            var packingList = await _packingListSLService.GetPackingListAsync(docEntry);

            if (packingList == null)
                return null;
            
            if (!packingList.IsCanCancelled())
                throw new ArgumentException("Não é possível remover lista com itens");
            
            await _packingListSLService.RemovePackingListAsync(docEntry);

            return "ok";
        }

        public byte[]? GetPdfPackingList(long packingListEntry)
        {
            var pl = _packingListSLService.GetPackingListAsync(packingListEntry).Result;

            if (pl == null)
                return null;

            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 }
            };
            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = TemplateGenerator.GetHTMLString(pl),
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet =  Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") },
                HeaderSettings = { FontName = "Arial", FontSize = 12, Right = "Página [page] de [toPage]", Line = true },
                FooterSettings = { FontName = "Arial", FontSize = 12, Line = true, Center = "Cetro Maquinas" }
            };
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            return _converter.Convert(pdf);
        }

        public async Task SendOrders(long docEntry)
        {
            var packingList = await _ordersSLService.GetOrdersToDispachtInIntelipost(docEntry);

            if (packingList.value.Count == 0 || !packingList.value.Any())
            {
                throw new Exception("Não há itens no romaneio para despachar.");
            }

            foreach(var order in packingList.value)
            {
                try
                {
                   // await _packingListSLService.UpdateOrderDispached((long)order.DocEntry);

                    var (obj, carrier) = await BuildOrderIntelipost(order, false);

                    if (obj != null)
                    {
                        await _apiIntelipost.ShippedOrderOnIntelipost(obj);
                    }
                }
                catch (Exception ex)
                {
                    continue;
                }

            }

            await _packingListSLService.UpdateDateDispatchPackingListAsync(docEntry);
        }

        private async Task<(OrderIntelipost, string)> BuildOrderIntelipost(OrderDocument orderSAP, bool resend)
        {
            try
            {
                if(orderSAP.BPL_IDAssignedToInvoice != 1)
                {
                    return (null, null);
                }

                var invoiceSL = await _ordersSLService.GetInfoDetails(orderSAP.DocNum);

                if (!invoiceSL.value.Any() || invoiceSL.value.FirstOrDefault().U_CT_Intelipost == null)
                {
                    return (null,null);
                }

                var orderIntelipost = new OrderIntelipost();
                string store = "VTEX";

                if (orderSAP.U_CT_Store == "9")
                    store = "Sales Force";

                if (orderSAP.U_CT_Store == "9")
                    store = "Sales Force";

                var deliveryMethod = invoiceSL.value.FirstOrDefault().U_CT_Intelipost.Value;

                if (orderSAP?.U_CT_Method == "PAC")
                {
                    deliveryMethod = 1;
                }

                orderIntelipost.Order_number = resend == true ? "RES-" + orderSAP.NumAtCard : orderSAP.NumAtCard;
                orderIntelipost.Shipment_order_type = resend == true ? "RESEND" : "NORMAL";
                orderIntelipost.Delivery_method_id = deliveryMethod;
                orderIntelipost.Parent_shipment_order_number = resend == true ? orderSAP.NumAtCard : orderSAP.DocEntry.ToString();
                orderIntelipost.Sales_order_number = orderSAP.NumAtCard;
                orderIntelipost.Created = DateTime.Now;
                orderIntelipost.Estimated_delivery_date = DateTime.Parse(orderSAP.DocDueDate);
                orderIntelipost.Sales_channel = store;
                orderIntelipost.Origin_zip_code = "17052080";
                orderIntelipost.Origin_warehouse_code = "02";
                orderIntelipost.End_customer = new EndCustomer();
                orderIntelipost.End_customer.First_name = orderSAP.CardName.Split(" ").FirstOrDefault();
                orderIntelipost.End_customer.Last_name = orderSAP.CardName.Split(" ").LastOrDefault();
                orderIntelipost.End_customer.Email = invoiceSL.value?.FirstOrDefault()?.Email;
                orderIntelipost.End_customer.Phone = invoiceSL.value?.FirstOrDefault()?.Phone1;
                orderIntelipost.End_customer.Cellphone = invoiceSL.value?.FirstOrDefault()?.Cellular;
                orderIntelipost.End_customer.Shipping_country = "Brasil";
                orderIntelipost.End_customer.Shipping_state = orderSAP.AddressExtension.ShipToState;
                orderIntelipost.End_customer.Shipping_city = orderSAP.AddressExtension.ShipToCity;
                orderIntelipost.End_customer.Shipping_address = orderSAP.AddressExtension.ShipToStreet;
                orderIntelipost.End_customer.Shipping_number = orderSAP.AddressExtension.ShipToStreetNo;
                orderIntelipost.End_customer.Shipping_zip_code = orderSAP.AddressExtension.ShipToZipCode;
                orderIntelipost.End_customer.Shipping_quarter = orderSAP.AddressExtension.ShipToBlock;
                orderIntelipost.End_customer.Shipping_additional = orderSAP.AddressExtension.ShipToBuilding;
                orderIntelipost.End_customer.Shipping_reference = orderSAP.AddressExtension.ShipToBuilding;

                if (orderSAP.CardCode.StartsWith("C"))
                {       
                    orderIntelipost.End_customer.Federal_tax_payer_id = orderSAP?.TaxExtension?.TaxId4?.Trim(new Char[] { ' ', '*', '.', '-' });
                    orderIntelipost.End_customer.Is_company = false ;
                }

                if (orderSAP.CardCode.StartsWith("F"))
                {
                    orderIntelipost.End_customer.Federal_tax_payer_id = orderSAP?.TaxExtension?.TaxId0?.Trim(new Char[] { ' ', '*', '.', '-' });
                    orderIntelipost.End_customer.Is_company = false;
                }
                
                orderIntelipost.Shipment_order_volume_array = new List<ShipmentOrderVolumeArray>();
                var products = new List<ProductIP>();

                foreach (var order in orderSAP?.DocumentLines?.ToList())
                {
                    products.Add(new ProductIP
                    {
                        Price = order.Price,
                        Description = order.ItemDescription,
                        Sku = order.ItemCode,
                        Category = "Máquinas",
                        Quantity = (int)order.Quantity
                    });
                };

                var invoice = new ShipmentOrderVolumeInvoice();

                if (invoiceSL.value.FirstOrDefault().Serial.HasValue)
                {
                    invoice.Invoice_series = (invoiceSL.value?.FirstOrDefault()?.SeriesStr == "" ? "01" : invoiceSL.value?.FirstOrDefault()?.SeriesStr) ?? "01";
                    invoice.Invoice_number = invoiceSL.value.FirstOrDefault().Serial.ToString() ?? "0001";
                    invoice.Invoice_key = invoiceSL.value.FirstOrDefault().U_ChaveAcesso ?? "";
                    invoice.Invoice_date = DateTime.Now;
                    invoice.Invoice_total_value = orderSAP.DocTotal;
                    invoice.Invoice_products_value = orderSAP.DocTotal;

                    orderIntelipost.Shipment_order_volume_array.Add(new ShipmentOrderVolumeArray
                    {
                        Name = "",
                        Tracking_code = orderSAP.U_CT_TrackingCode ?? "",
                        Shipment_order_volume_number = 1,
                        Volume_type_code = "BOX",
                        Products_quantity = 1,
                        Products_nature = "products",
                        Products = products,
                        Shipment_order_volume_invoice = invoice
                    });
                }
                else
                {
                    orderIntelipost.Shipment_order_volume_array.Add(new ShipmentOrderVolumeArray
                    {
                        Name = "",
                        Tracking_code = orderSAP.U_CT_TrackingCode ?? "",
                        Shipment_order_volume_number = 1,
                        Volume_type_code = "BOX",
                        Products_quantity = 1,
                        Products_nature = "products",
                        Products = products
                    });
                }

                orderIntelipost.Content_declaration = new ContentDeclaration();

                orderIntelipost.Content_declaration.Content_declaration_date = DateTime.Now.ToString("yyyy-MM-dd");
                orderIntelipost.Content_declaration.Content_declaration_number = orderSAP.NumAtCard;
                orderIntelipost.Content_declaration.Content_declaration_total_value = orderSAP.DocTotal.ToString(CultureInfo.InvariantCulture);

                return (orderIntelipost, invoiceSL.value.FirstOrDefault().CardName);

            }
            catch (Exception ex)
            {
                _logger.LogError($"tracking - error={ex.Message} stackTrace={ex?.StackTrace}");
                return (null, null);
            }
        }

        public async Task PrintDanfe(string ip, int amountOfBoxes, string docEntry, string keyNfe)
        {
            var reference = docEntry == null ? keyNfe : docEntry;
            var labels = new List<string>();

            var order = await _packingListSLService.GetInfoNFEAsync(reference);

            if (!order.NF.Any())
                throw new Exception("Não encontrado pedido para NF/DocEntry informado.!");

            Connection thePrinterConn = new TcpConnection(ip, TcpConnection.DEFAULT_ZPL_TCP_PORT);
            thePrinterConn.Open();

            for (int i = 1; i <= amountOfBoxes; i++)
            {

                    if (!string.IsNullOrEmpty(order.NF.FirstOrDefault().U_WM_TagDanfe))
                    {
                        var valueBytes = Convert.FromBase64String(order.NF.FirstOrDefault().U_WM_TagDanfe.Replace("%vol%", i.ToString() + " / " + amountOfBoxes.ToString()));
                        var labelPrint = Encoding.UTF8.GetString(valueBytes);
                        thePrinterConn.Write(Encoding.UTF8.GetBytes(labelPrint));
                    }

                    if (!string.IsNullOrEmpty(order.NF.FirstOrDefault().U_CT_Label))
                    {
                        var valueBytes = Convert.FromBase64String(order.NF.FirstOrDefault().U_CT_Label.Replace("%vol%", i.ToString() + " / " + amountOfBoxes.ToString()));
                        var labelPrint = Encoding.UTF8.GetString(valueBytes);
                        thePrinterConn.Write(Encoding.UTF8.GetBytes(labelPrint));
                    }
            }

            thePrinterConn.Close();
        }

        public async Task AddItemIntelipostAsync(string serial, bool resend)
        {
            var invoiceEntry = await _packingListSLService.GetInvoiceEntryBySerialAsync(serial);

            if (invoiceEntry == default)
                throw new KeyNotFoundException($"Chave {serial} não encontrada");

            var order = await _packingListSLService.GetInfoNFEAsync(invoiceEntry.AccessKey);

            var orderSAP = await _ordersSLService.GetOrderToSendInIntelipost((long)order?.NF?.FirstOrDefault()?.DocEntry);

            var (obj, carrierName) = await BuildOrderIntelipost(orderSAP?.value?.FirstOrDefault(), resend);

            if (obj != null)
            {
                var trackingUrl = await _apiIntelipost.CreateOrderonIntelipost(obj);
                await _apiIntelipost.ReadyForShipmentOrderOnIntelipost(obj.Order_number);
                await _apiIntelipost.ShippedOrderOnIntelipost(obj);

                var objVTEX = new TrackingVtex
                {
                    trackingNumber = orderSAP?.value?.FirstOrDefault()?.U_CT_TrackingCode,
                    trackingUrl = trackingUrl?.content?.tracking_url.ToString(),
                    courier = carrierName,
                    dispatchedDate = DateTime.Now,
                    invoiceNumber = invoiceEntry.SequenceSerial.ToString(),
                    orderId = obj.Order_number
                };

                JsonSerializerOptions options = new()
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                _publisher.Publish(JsonSerializer.Serialize(objVTEX, options),
                    $"storeId={orderSAP?.value?.FirstOrDefault().U_CT_Store}",
                   _configuration.GetSection("Rabbitmq:Shipped.Created:ExchangeName").Value);
            }
        }
    }
}