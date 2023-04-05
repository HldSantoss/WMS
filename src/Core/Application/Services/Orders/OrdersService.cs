using System;
using System.Globalization;
using Domain.Entities.Orders;
using Domain.Services;
using Domain.ValueObject;
using Infra.ServiceLayer.Interfaces;

namespace Application.Services.Orders
{
    public class OrdersService : IOrdersService
    {
        private readonly IOrdersSLService _serviceLayerAdapter;
        public OrdersService(IOrdersSLService serviceLayerAdapter)
        {
            _serviceLayerAdapter = serviceLayerAdapter;
        }

        public async Task<IEnumerable<SalesOrdersTracking>> SalesOrdersTrackingAsync(int branchId)
        {
            var orders = await _serviceLayerAdapter.GetSummarySalesOrder(branchId);
            var listOrder = new List<SalesOrdersTracking>();

            listOrder.Add(RangeOrder(1, DateTime.Now, "Created", 0));
            listOrder.Add(RangeOrder(2, DateTime.Now, "CanTax", 0));
            listOrder.Add(RangeOrder(3, DateTime.Now, "OutOfStock", 0));
            listOrder.Add(RangeOrder(4, DateTime.Now, "Replenish", 0));
            listOrder.Add(RangeOrder(5, DateTime.Now, "CanPick", 0));
            listOrder.Add(RangeOrder(6, DateTime.Now, "Picking", 0));
            listOrder.Add(RangeOrder(7, DateTime.Now, "CanCheckout", 0));
            listOrder.Add(RangeOrder(8, DateTime.Now, "Checkingout", 0));
            listOrder.Add(RangeOrder(9, DateTime.Now, "SavePicking", 0));
            listOrder.Add(RangeOrder(10, DateTime.Now, "CanPacking", 0));
            listOrder.Add(RangeOrder(11, DateTime.Now, "Packing", 0));
            listOrder.Add(RangeOrder(12, DateTime.Now, "Shipped", 0));

            orders.ToList().ForEach(e =>
            {
                listOrder.Add(RangeOrder(11, DateTime.ParseExact(e.LimitDate, "yyyyMMdd", CultureInfo.InvariantCulture), e.Status, e.Qty));
            });


            listOrder.ToList().ForEach(s =>
            {
                s.Status = TranslateSummary(s.Status);
            });

            var sorterList = listOrder.ToList().OrderBy(x => x.Weight).ToList();

            var summary = sorterList.GroupBy(x => x.Status)
                 .Select(s => new SalesOrdersTracking(s.Key,
                                                      s.Sum(c => c.D0),
                                                      s.Sum(c => c.D1),
                                                      s.Sum(c => c.D2),
                                                      s.Sum(c => c.D3),
                                                      s.Sum(c => c.DX))).ToList();   
            return summary;
        }

        public async Task<IEnumerable<SalesOrdersTracking>> SalesOrdersByUfTrackingAsync()
        {
            var orders = await _serviceLayerAdapter.GetSummarySalesOrderByUf();
            var listOrder = new List<SalesOrdersTracking>();

            orders.ToList().ForEach(e =>
            {
                listOrder.Add(RangeOrder(28,DateTime.ParseExact(e.LimitDate, "yyyyMMdd", CultureInfo.InvariantCulture), e.Uf, e.Qty));
            });

            listOrder.Add(RangeOrder(1,DateTime.Now, "AC", 0));
            listOrder.Add(RangeOrder(2,DateTime.Now, "AL", 0));
            listOrder.Add(RangeOrder(3,DateTime.Now, "AP", 0));
            listOrder.Add(RangeOrder(4,DateTime.Now, "AM", 0));
            listOrder.Add(RangeOrder(5,DateTime.Now, "BA", 0));
            listOrder.Add(RangeOrder(6,DateTime.Now, "CE", 0));
            listOrder.Add(RangeOrder(7,DateTime.Now, "ES", 0));
            listOrder.Add(RangeOrder(8,DateTime.Now, "GO", 0));
            listOrder.Add(RangeOrder(9,DateTime.Now, "MA", 0));
            listOrder.Add(RangeOrder(10,DateTime.Now, "MT", 0));
            listOrder.Add(RangeOrder(11,DateTime.Now, "MS", 0));
            listOrder.Add(RangeOrder(12,DateTime.Now, "MG", 0));
            listOrder.Add(RangeOrder(13,DateTime.Now, "PA", 0));
            listOrder.Add(RangeOrder(14,DateTime.Now, "PB", 0));
            listOrder.Add(RangeOrder(15,DateTime.Now, "PR", 0));
            listOrder.Add(RangeOrder(16,DateTime.Now, "PE", 0));
            listOrder.Add(RangeOrder(17,DateTime.Now, "PI", 0));
            listOrder.Add(RangeOrder(18,DateTime.Now, "RJ", 0));
            listOrder.Add(RangeOrder(19,DateTime.Now, "RN", 0));
            listOrder.Add(RangeOrder(20,DateTime.Now, "RS", 0));
            listOrder.Add(RangeOrder(21,DateTime.Now, "RO", 0));
            listOrder.Add(RangeOrder(22,DateTime.Now, "RR", 0));
            listOrder.Add(RangeOrder(23,DateTime.Now, "SC", 0));
            listOrder.Add(RangeOrder(24,DateTime.Now, "SP", 0));
            listOrder.Add(RangeOrder(25,DateTime.Now, "SE", 0));
            listOrder.Add(RangeOrder(26,DateTime.Now, "TO", 0));
            listOrder.Add(RangeOrder(27, DateTime.Now, "DF", 0));

            var summary = listOrder.GroupBy(x => x.Status)
                 .Select(s => new SalesOrdersTracking(s.Key,
                                                      s.Sum(c => c.D0),
                                                      s.Sum(c => c.D1),
                                                      s.Sum(c => c.D2),
                                                      s.Sum(c => c.D3),
                                                      s.Sum(c => c.DX)));
            return summary;
        }

        public static SalesOrdersTracking RangeOrder(int weight, DateTime date, string status, int qty)
        {
            if (date.Date >= DateTime.Now.Date)
            {
                return new SalesOrdersTracking(status, weight, qty, 0, 0, 0, 0);
            }

            if (date.Date == DateTime.Now.AddDays(-1).Date)
            {
                return new SalesOrdersTracking(status, weight, 0, qty, 0, 0, 0);
            }

            if (date.Date == DateTime.Now.AddDays(-2).Date)
            {
                return new SalesOrdersTracking(status, weight, 0, 0, qty, 0, 0);
            }

            if (date.Date == DateTime.Now.AddDays(-3).Date)
            {
                return new SalesOrdersTracking(status, weight, 0, 0, 0, qty, 0);
            }

            if (date.Date < DateTime.Now.AddDays(-3).Date)
            {
                return new SalesOrdersTracking(status, weight, 0, 0, 0, 0, qty);
            }

            return new SalesOrdersTracking(status, weight,0, 0, 0, 0, 0);
        }

        public static (DateTime,DateTime) DefinedRange(String range)
        {
            return range switch
            {
                "D0" => (DateTime.Now.AddDays(-1).Date, DateTime.Now.AddDays(20).Date), //D0 > ONTEM < AMANHA
                "D1" => (DateTime.Now.AddDays(-2).Date, DateTime.Now.Date), // D1 > ANTEONTEM < HOJE
                "D2" => (DateTime.Now.AddDays(-3).Date, DateTime.Now.AddDays(-1).Date), // D2 > ANTEANTEONTEM < ONTEM
                "D3" => (DateTime.Now.AddDays(-4).Date, DateTime.Now.AddDays(-2).Date), //D3 > ANTE ANTE ANTEONTEM < ANTEONTEM
                "DX" => (DateTime.Now.AddDays(-25).Date, DateTime.Now.AddDays(-3).Date), //DX > 15 DIAS < 

                _ => (DateTime.Now.Date, DateTime.Now.AddDays(-25).Date)
            };
        }

        public static string TranslateSummary(string status)
        {
            return status switch
            {
                "CanTax" => "Liberado para Fiscal",
                "CanPick" => "Liberado para Picking",
                "OutOfStock" => "Sem estoque",
                "Replenish" => "Aguardando Abastecimento",
                "Picking" => "Picking em andamento",
                "CanCheckout" => "Liberado para Checkout",
                "Checkingout" => "Checkout em andamento",
                "Created" => "Criado",
                "SavePicking" => "Picking Salvo",
                "CanPacking" => "Liberado para Packing",
                "Packing" => "Packing em Andamento",
                "Shipped" => "Despachado",
                _ => "Outros"
            };
        }

        public static string TranslateDetails(String status)
        {
            return status switch
            {
                "Liberado para Fiscal" =>  "CanTax",
                "Liberado para Picking" => "CanPick" ,
                "Sem estoque" => "OutOfStock",
                "Aguardando Abastecimento" => "Replenish",
                "Picking em andamento" => "Picking",
                "Liberado para Checkout" => "CanCheckout",
                "Checkout em andamento" => "Checkingout",
                "Criado" => "Created",
                "Picking Salvo" => "SavePicking",
                "Liberado para Packing" => "CanPacking",
                "Packing em Andamento" => "Packing",
                "Despachado" => "Shipped",
                _ => "Outros"
            };
        }

        public async Task<IEnumerable<OrderDetails>> SalesOrdersDetailsAsync(string range, string status, int branchId)
        {
            (DateTime startAt, DateTime finishAt) = DefinedRange(range);

            var statusWms = TranslateDetails(status);

            var orders =  await _serviceLayerAdapter.GetSummarySalesOrderDetails(startAt.ToString("yyyy-MM-dd"), finishAt.ToString("yyyy-MM-dd"), statusWms, branchId);

            return orders.OrdersDetails;
        }

        public async Task<OrdersByCardCode> SalesOrdersByClientAsync(string reference)
        {
            string cardCode = "";

            cardCode = await GetReference(reference);

            if (string.IsNullOrEmpty(cardCode))
            {
                var cardCodeList = await _serviceLayerAdapter.GetOrderClientByReference(reference);

                cardCode = cardCodeList?.value?.FirstOrDefault()?.CardCode;
            }

            if (string.IsNullOrEmpty(cardCode))
                throw new Exception();

            return await _serviceLayerAdapter.GetOrdersByCardCode(cardCode);

        }

        private async Task<string> GetReference(string reference)
        {
            try
            {
                string unformattedCPF;
                string formattedCPF;
                string unformattedCNPJ;
                string formattedCNPJ;
                string unformattedOrder;
                string formattedOrder;


                var dotlessTextSplit = reference.Split(".");
                var dotlessText = string.Join("", dotlessTextSplit.ToList()).Trim();

                var textWithoutDash = dotlessText.Split("-");
                var spacesRemove = string.Join("", textWithoutDash).Trim();

                if(spacesRemove.Length > 10)
                {
                    formattedCPF = FormattedCPF(spacesRemove);
                }
                else
                {
                    formattedCPF = spacesRemove;
                }

                if (spacesRemove.Length > 13)
                {
                    formattedCNPJ = FormattedCNPJ(spacesRemove);
                }
                else
                {
                    formattedCNPJ = spacesRemove;
                }
                
                var cardCodeByCPF = await _serviceLayerAdapter.GetOrderClientByCPF(reference, formattedCPF);

                if (cardCodeByCPF?.value.Count() > 0)
                    return cardCodeByCPF.value.FirstOrDefault().CardCode;

                var cardCodeByCNPJ = await _serviceLayerAdapter.GetOrderClientByCPF(reference, formattedCNPJ);

                if (cardCodeByCNPJ.value.Count() > 0)
                    return cardCodeByCNPJ.value.FirstOrDefault().CardCode;

                return null;

            }
            catch (Exception ex)
            {

                return null;
            }
        }

        public async Task CancelOrderByDocEntryAsync(long docEntry)
        {
            await _serviceLayerAdapter.CancelOrderByDocEntry(docEntry);
        }

        public async Task<IEnumerable<SalesOrderStore>> SalesOrdersTrackingByStoreAsync(int branchId)
        {
            var orders = await _serviceLayerAdapter.GetSummarySalesOrderByStore(branchId);
            var listOrder = new List<SalesOrderStore>();

            orders.value?.ToList().ForEach(e =>
            {
                listOrder.Add(RangeWmsStatus(e.Store ,e.Status, e.Qty));
            });

            var summary = listOrder.GroupBy(x => x.Store)
               .Select(s => new SalesOrderStore(s.Key,
                                                    s.Sum(c => c.Created),
                                                    s.Sum(c => c.CanPick),
                                                    s.Sum(c => c.CanTax),
                                                    s.Sum(c => c.Picking),
                                                    s.Sum(c => c.CanCheckout),
                                                    s.Sum(c => c.Checkingout),
                                                    s.Sum(c => c.SavePicking),
                                                    s.Sum(c => c.Replenish),
                                                    s.Sum(c => c.CanPacking),
                                                    s.Sum(c => c.Packing)
                                                    ));
            return summary;
        }

        private static SalesOrderStore RangeWmsStatus(string store, string status, int qty)
        {
            if (status == "Created")
            {
                return new SalesOrderStore(store,qty, 0, 0, 0,0, 0,0,0,0,0);
            }

            if (status == "CanPick")
            {
                return new SalesOrderStore(store, 0, qty, 0, 0, 0, 0, 0, 0,0,0);
            }

            if (status == "CanTax")
            {
                return new SalesOrderStore(store, 0, 0, qty, 0, 0, 0, 0, 0, 0, 0);
            }

            if (status == "Picking")
            {
                return new SalesOrderStore(store, 0, 0, 0, qty, 0, 0, 0, 0,0, 0);
            }

            if (status == "CanCheckout")
            {
                return new SalesOrderStore(store, 0, 0, 0, 0, qty, 0, 0, 0, 0,0);
            }

            if (status == "Checkingout")
            {
                return new SalesOrderStore(store, 0, 0, 0, 0, 0,qty, 0, 0, 0,0);
            }

            if (status == "SavePicking")
            {
                return new SalesOrderStore(store, 0, 0, 0, 0, 0, 0, qty, 0,0, 0);
            }

            if (status == "Replenish")
            {
                return new SalesOrderStore(store, 0, 0, 0, 0, 0, 0, 0, qty, 0, 0);
            }

            if (status == "CanPacking")
            {
                return new SalesOrderStore(store, 0, 0, 0, 0, 0, 0, 0, 0, qty,0);
            }

            if (status == "Packing")
            {
                return new SalesOrderStore(store, 0, 0, 0, 0, 0, 0, 0, 0,0, qty);
            }

            return new SalesOrderStore(store, 0, 0, 0, 0, 0, 0, 0, 0, 0,0);
        }

        string FormattedCPF(string cpf) => cpf.Substring(0, 3) + "." + cpf.Substring(3, 3) + "." + cpf.Substring(6, 3) + "-" + cpf.Substring(9, 2);
        string FormattedCNPJ(string cnpj) => cnpj.Substring(0, 2) + "." + cnpj.Substring(2, 3) + "." + cnpj.Substring(5, 3) + "/" + cnpj.Substring(8, 4) + "-" + cnpj.Substring(12, 2);

    }
}

