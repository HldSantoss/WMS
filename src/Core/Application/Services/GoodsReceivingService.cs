using Domain.Entities.ReceiptOfGoods;
using Domain.Services;
using Infra.ServiceLayer.Interfaces;

namespace Application.Services
{
    public class GoodsReceivingService : IGoodsReceivingService
    {
        private readonly IGoodsReceivingSLService _goodsReceivingSLService;

        public GoodsReceivingService(IGoodsReceivingSLService goodsReceivingSLService)
        {
             _goodsReceivingSLService = goodsReceivingSLService;
        }

        public async Task FinishReceivingAsync(string keyAccess, long docEntry, string userName)
        {
            var purchase = await _goodsReceivingSLService.GetPurchaseOrderByKeyAcessAsync(keyAccess) ?? throw new ArgumentNullException("Houve um erro ao recuperar as informações, tente novamente");

            var seriesItem = await _goodsReceivingSLService.GetSeriesItemAsync(docEntry) ?? throw new Exception("DocEntry não encontrada");

            var status = BuildStatus(seriesItem, purchase);

            var deliveryNotes = BuildDeliveryNotes(seriesItem, purchase);

            var purchaseOrderUpdate = new PurchaseOrderUpdate(userName, status);

            await _goodsReceivingSLService.UpdateItemByPurchaseOrder(purchaseOrderUpdate, docEntry);

            if (status == "CanTest")
            {
                await _goodsReceivingSLService.CreateDeliveryNote(deliveryNotes);
            }
        }

        private static string BuildStatus(SeriesItem seriesItem, GoodsReceiving purchase)
        {
            string status = "CanTest";

            purchase.Item.ToList().ForEach(e =>
            {
                var item = seriesItem.Items.Where(p => p.MaterialCode == e.ItemCode && p.Confirmed == "Y").Count();

                if (e.Quantity != double.Parse(e.QuantityReceiving ?? "0"))
                    status = "Divergent";

                if (e.Quantity != item && e.ManSerNum == "Y")
                    status = "Divergent";
            });

            return status;
        }

        private static DeliveryNotesCreated BuildDeliveryNotes(SeriesItem seriesItem, GoodsReceiving purchase)
        {
            var deliveryNotes = new DeliveryNotesCreated();

            var keyAcess =  purchase.Item.FirstOrDefault().Keyaccess;
            // Cria Header Deliver
            deliveryNotes.BPL_IDAssignedToInvoice = purchase.Item.FirstOrDefault().Bpl; 
            deliveryNotes.DocDate = DateTime.Now.ToString("yyyy/MM/dd");
            deliveryNotes.DocDueDate = DateTime.Now.ToString("yyyy/MM/dd");
            deliveryNotes.NumAtCard = keyAcess.Substring(25, 9);
            deliveryNotes.SequenceCode = -2;
            deliveryNotes.SequenceSerial = Convert.ToInt64(keyAcess.Substring(25,9));
            deliveryNotes.SeriesString = keyAcess.Substring(22, 3);
            deliveryNotes.SubSeriesString = "";
            deliveryNotes.SequenceModel = "39";
            deliveryNotes.DocumentLines = new List<DocumentLine>();
            // Cria Itens Delivery
            var itemLine = seriesItem.Items.DistinctBy(m => m.MaterialCode).ToList();

            itemLine.ForEach(i =>
            {
                var serie = new List<SerialNumber>();

                var item = purchase.Item.Where(x => x.ItemCode == i.MaterialCode).ToList();

                if (i.SeriesControlled == "Y")
                {
                    seriesItem.Items.Where(it => it.MaterialCode == i.MaterialCode).ToList().ForEach(i =>
                    {
                        serie.Add(new SerialNumber
                        {
                            InternalSerialNumber = i.Serie,
                            BaseLineNumber = item.FirstOrDefault().LineNum,
                            Quantity = 1
                        });
                    });
                }

                deliveryNotes.DocumentLines.Add(new DocumentLine
                {
                    ItemCode = i.MaterialCode,
                    Quantity = item.Sum(p => p.Quantity),
                    BaseType = 22,
                    BaseEntry = (int)seriesItem.DocEntry,
                    BaseLine = item.FirstOrDefault().LineNum,
                    SerialNumbers = serie
                });
            });

            return deliveryNotes;
        }


        public async Task ReceivingItemBySkuAsync(ReceivingItemWms item, long docEntry)
        {
            var purchaseOrder = await _goodsReceivingSLService.GetQuantityReceivingPurchaseOrderByDocEntryAsync(docEntry);
            int qty = 0;

            if (purchaseOrder.Value.Count() == 0)
            {
                throw new Exception("Pedido não encontrado.");
            }

            var itemUpdate = purchaseOrder.Value.Where(x => x.ItemCode == item.MaterialCode).FirstOrDefault();

            if (itemUpdate is null)
            {
                throw new Exception("Item não encontrado.");
            }

            if (item.QuantityReceiving > 0)
            {
                qty = int.Parse(itemUpdate.QuantityReceiving ?? "0") + item.QuantityReceiving;
            }

            if(qty > itemUpdate.Quantity)
            {
                qty = (int)itemUpdate.Quantity;
            } 

            var documentLines = new List<PurchaseOrderUpdateDocumentLine>()
            {
                new PurchaseOrderUpdateDocumentLine(itemUpdate.LineNum, qty.ToString())
            };

            var purchaseOrderUpdate = new PurchaseOrderUpdate("teste", "Confer", documentLines); // Virá do token;
            await _goodsReceivingSLService.UpdateItemByPurchaseOrder(purchaseOrderUpdate, docEntry);
        }

        public async Task<ReceivingWms> StartReceivingAsync(string keyAccess, string userName)
        {
            var purchase = await _goodsReceivingSLService.GetPurchaseOrderByKeyAcessAsync(keyAccess) ?? throw new ArgumentNullException("Houve um erro ao recuperar as informações, tente novamente");

            ValidInitializeReceiving(purchase);

            var receiving = CreateObject(purchase, keyAccess);

            var seriesSaving = await _goodsReceivingSLService.GetSeriesItemAsync((long)purchase.Item.First().DocEntry);
            if (seriesSaving != null)
            {
                var objSaving = GetObjectSeries(receiving, seriesSaving);

                return objSaving;
            }

            var series = CreateObjectSeries(receiving, receiving.DocEntry, userName);
            var purchaseOrderUpdate = new PurchaseOrderUpdateHeader(userName, "Confer");

            await _goodsReceivingSLService.UpdateHeaderPurchaseOrder(purchaseOrderUpdate, (long)purchase.Item.First().DocEntry);
            await _goodsReceivingSLService.PostSeriesItemAsync(series);

            return receiving;
        }

        public async Task UndoItemAsync(Item item, long docEntry) =>
            await _goodsReceivingSLService.UpdateItemByPurchaseOrder(
                new PurchaseOrderUpdate(
                    new List<PurchaseOrderUpdateDocumentLine>
                    {
                        new PurchaseOrderUpdateDocumentLine(item.LineNum, "0")
                    }), docEntry);

        public async Task GetEquipamentTestAsync(string serialNumber, string userName)
        {
            // var purchase = await _serviceLayerAdapter.Get

            // if (purchase == null || !purchase.Item.Any())
            //     throw new Exception("Chave de acesso não encontrada");

            // var headerPurchase = purchase.Item.First();
            //  var purchaseUpdate = new PurchaseOrderUpdateHeader(userName, "Testing");

            //  await _serviceLayerAdapter.UpdateHeaderPurchaseOrder(purchaseUpdate, headerPurchase.DocEntry);

        }

        public async Task RecordSeriesItemReceivingAsync(SeriesItem seriesItem, long docEntry)
        {
            if (!seriesItem.DocEntry.HasValue)
                throw new Exception("Número do docEntry precisa ser informado.");

            var series = await _goodsReceivingSLService.GetSeriesItemAsync(docEntry);

            await _goodsReceivingSLService.UpdateSeriesItemLineAsync(seriesItem, docEntry);
        }

        private void ValidInitializeReceiving(GoodsReceiving receiving)
        {
            if (!receiving.Item.Any())
                throw new Exception("Chave de acesso não encontrada");

            if (receiving.Item.First().Canceled == "Y")
                throw new Exception("O Pedido está cancelado, por favor procure o departamento comercial");

            if (receiving.Item.First().DocStatus != "O")
                throw new Exception("O Pedido já foi finalizado, por favor procure o departamento comercial");

            var status = receiving.Item.First().Status;
            if (status == "CanTax")
                throw new Exception("O Pedido está aguardando validação fiscal, procure o setor Fiscal.");

            if (status != "CanConfer" && status != "Confer" && status != "Divergent")
                throw new Exception("O Pedido já foi finalizado ou está em teste e não é possível realizar o recebimento.");
        }

        private ReceivingWms CreateObject(GoodsReceiving receiving, string nfekey)
        {
            var header = receiving.Item.First();
            var receivingWms = new ReceivingWms(nfekey, header.CardCode, header.DocEntry, null, header.Status, null, header.Comments, null, new List<Item>());

            receivingWms.Items = new List<Item>();

            var ln = 1;
            var lnGeneral = 1;

            foreach (var item in receiving.Item)
            {
                var qty = receiving.Item.Sum(x => x.Quantity);

                if (item.ManSerNum == "Y")
                {
                    for (int lineNum = 0; lineNum < item.Quantity; lineNum++)
                    {
                        receivingWms.Items.Add(new Item(item.ItemCode,
                            item.Dscription,
                            lnGeneral,
                            item.CodeBars,
                            DateTime.Now.ToString("yyyyMMddHHmm") + item.DocEntry + lnGeneral, item.Quantity,
                            double.Parse(item.QuantityReceiving ?? "0"),
                            0,
                            item.ManSerNum,
                            ln + "/" + item.Quantity)); ;

                        ++ln;
                        ++lnGeneral;
                    }

                    ln = 1;
                }
                else
                {
                    receivingWms.Items.Add(new Item(item.ItemCode,
                           item.Dscription,
                           lnGeneral,
                           item.CodeBars,
                           item.ItemCode,
                           item.Quantity,
                           double.Parse(item.QuantityReceiving ?? "0"),
                           0,
                           item.ManSerNum,
                           ln + "/" + item.Quantity));

                    ln = 1;
                    ++lnGeneral;
                }

            }

            return receivingWms;
        }

        private SeriesItem CreateObjectSeries(ReceivingWms receiving, long docEntry, string userName)
        {
            SeriesItem series = new SeriesItem(receiving.DocEntry.ToString(), "Recebimento de Mercadoria", DateTime.Now.ToString("yyyy-MM-dd"), receiving.DocEntry, 22, new List<SeriesItemLine>());

            // User Virá do Token
            for (int i = 0; i < receiving.Items.ToList().Count(); i++)
            {
                series.Items.Add(new SeriesItemLine((i + 1), DateTime.Now.ToString("yyyy-MM-dd"), userName, receiving.Items[i].Series, false.ToString(), "", "N", receiving.Items[i].MaterialCode, receiving.Items[i].SerialControlled));
            }

            return series;
        }

        private ReceivingWms GetObjectSeries(ReceivingWms receiving, SeriesItem seriesItem)
        {
            var ln = 1;

            receiving.Items.ForEach(i =>
            {

                i.Series = seriesItem.Items.Where(p => p.Line == ln).Select(p => p.Serie).FirstOrDefault();
                ln += 1;
            });

            return receiving;
        }

        public async Task<SeriesItem?> DanfeEquipamentAsync(string keyAccess)
        {
            var purchase = await _goodsReceivingSLService.GetPurchaseOrderByKeyAcessAsync(keyAccess);

            if (purchase == null || !purchase.Item.Any())
                throw new Exception("Chave de acesso não encontrada");

            var x = await _goodsReceivingSLService.GetSeriesItemAsync((long)purchase.Item.First().DocEntry);
            return x;
        }

        public async Task ReceivingItemBySerieAsync(ReceivingItemBySerie item, long docEntry, int lineNum)
        {
            var itemHeader = new List<SeriesItemLine>();

            if (item is null)
            {
                throw new ArgumentNullException("O Objeto não pode ser nulo.");
            }

            var purchaseOrder = await _goodsReceivingSLService.GetQuantityReceivingPurchaseOrderByDocEntryAsync(docEntry);

            if (purchaseOrder.Value.Count() == 0)
            {
                throw new Exception("Pedido não encontrado.");
            }

            var itemUpdate = purchaseOrder.Value.Where(x => x.ItemCode == item.MaterialCode).FirstOrDefault();

            if (itemUpdate is null)
            {
                throw new Exception("Item não encontrado.");
            }

            var seriesSaving = await _goodsReceivingSLService.GetSeriesItemAsync(docEntry);

            if (seriesSaving is null)
            {
                throw new Exception("Referencia não encontrada.");
            }

            var itemSerie = seriesSaving.Items.Where(s => s.Serie == item.Serie).FirstOrDefault();

            if (itemSerie is null)
            {
                throw new Exception("Serie não existe.");
            }

            itemHeader.Add(new SeriesItemLine(
                itemSerie.Line,
                DateTime.Now.ToString("yyyy-MM-dd"),
                item.User,
                item.Serie,
                "false",
                "",
                "Y",
                item.MaterialCode,
                itemUpdate.ManSerNum));

            var itemReceiving = new SeriesItemHeader(itemHeader);

            await _goodsReceivingSLService.UpdateSeriesItemHeaderAsync(itemReceiving, docEntry);

            var qty = int.Parse(itemUpdate.QuantityReceiving ?? "0") + 1;

            if(qty > itemUpdate.Quantity)
            {
                var documentLines = new List<PurchaseOrderUpdateDocumentLine>()
                {
                   new PurchaseOrderUpdateDocumentLine(itemUpdate.LineNum,  int.Parse(itemUpdate.QuantityReceiving ?? "0").ToString())
                };

                var purchaseOrderUpdate = new PurchaseOrderUpdate("teste", "Confer", documentLines); // Virá do token;
                await _goodsReceivingSLService.UpdateItemByPurchaseOrder(purchaseOrderUpdate, docEntry);
            }
        }
    }
}