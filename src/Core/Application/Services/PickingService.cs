using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.Adapters;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Domain.Services;
using Infra.ServiceLayer.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Application.Services
{
    public class PickingService : IPickingService
    {
        private readonly IPickingSLService _pickingSLService;
        private readonly IPublisher _publisher;
        private readonly IConfiguration _configuration;
        private readonly ICreateStructure _createStructure;
        private readonly IActivityRepository _activityRepository;

        public PickingService(IPickingSLService pickingSLService, IPublisher publisher, IConfiguration configuration, ICreateStructure createStructure, IActivityRepository activityRepository)
        {
            _pickingSLService = pickingSLService;
            _publisher = publisher;
            _configuration = configuration;
            _createStructure = createStructure;

            _createStructure.CreateConsumerChannel(
                _configuration.GetSection("Rabbitmq:Invoice.Create:ExchangeName").Value,
                _configuration.GetSection("Rabbitmq:Invoice.Create:QueueName").Value,
                _configuration.GetSection("Rabbitmq:Invoice.Create:RouteKey").Value,
                true);
            _activityRepository = activityRepository;
        }

        public async Task<Picking> GetPicking(string userId)
        {
            var allUserGroups = await _pickingSLService.GetUsersPickingGroupAsync() ?? throw new ArgumentException("Nenhum grupo de picking existente.");

            var userGroups = from items in allUserGroups.PickingGroupItems
                          where items.Users.Contains(new PickingGroupUsers(userId))
                          select items.Code;

            if (!userGroups.Any())
                throw new ArgumentException($"O usuário {userId} não está em nenhum grupo de picking");

            var orderId = await _pickingSLService.GetNextPickingAsync(string.Join(",", userGroups.Select(p => "'" + p + "'"))) ?? throw new ArgumentException($"Nenhum picking disponível para o(s) grupo(s) {string.Join(",", userGroups.Select(p => "'" + p + "'"))}");
            var order = await _pickingSLService.GetPickingAsync(orderId) ?? throw new ArgumentNullException("Não possui picking para esse grupo");
            order.Carrier = order.TaxExtension.Carrier;

            await _pickingSLService.UpdatePickingStatusAsync(OrderStatusEnum.Picking.ToString(), orderId, userId);
            
            if (!string.IsNullOrWhiteSpace(order.TaxExtension.Carrier))
                order.CarrierName = await _pickingSLService.GetCarrierName(order.TaxExtension.Carrier);

            var orderValid = order.Items.Where(p => p.TreeType != "iSalesTree").ToList() ?? throw new ArgumentNullException("Não possui itens validos para coletar");

            orderValid.ForEach(p => p.CreateLabels(order.DocEntry, order.CarrierName,userId, order.Items.Count()));

            order.Items = orderValid;
            order.CreateLabelControl();

            await AllocateItemsPickBinsAsync(order);

            CultureInfo pt = new CultureInfo("pt-BR");
            Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR", false);
            var dateAjusted = DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss");

            await _activityRepository.SaveActivity(new Activity(dateAjusted, userId, WmsAction.StartPicking.ToString(), order.DocEntry.ToString(), order.BPLId.ToString())); ;
            return order;
        }

        public async Task<Picking?> ContinuePicking(long docEntry)
        {
            if (!await _pickingSLService.KeepGoingAsync(docEntry))
                return null;

            var pickingSaved = await _pickingSLService.GetSavedPickingAsync(docEntry);

            if (pickingSaved == null || !pickingSaved.Pickings.Any())
                return null;

            var picking = JsonSerializer.Deserialize<Picking>(pickingSaved.Pickings.First().U_Payload);

            await _pickingSLService.UpdatePickingStatusAsync(OrderStatusEnum.Picking.ToString(), docEntry, "");
            await _pickingSLService.RemovePickSavedAsync(pickingSaved.Pickings.First().DocEntry);

            return picking;
        }

        private async Task AllocateItemsPickBinsAsync(Picking order)
        {
            var bins = await _pickingSLService.GetBinsPickingAsync(order.DocEntry) ?? throw new ArgumentNullException("Nenhum item do pedido possui estoque");

            foreach (var item in order.Items)
            {
                var listBins = new List<BinLocations>();
                var tmpQtd = item.Quantity;

                foreach (var bin in bins.ItemsBins.Where(p => p.ItemCode == item.ItemCode).OrderBy(p => p.BinCode))
                {
                    item.ManBtchNum = bin.ManBtchNum;
                    item.ManSerNum = bin.ManSerNum;

                    var itemPickBin = new BinLocations(item.LineNum, -1, bin.BinCode, 0, bin.AbsEntry);

                    if (bin.OnHandQty >= tmpQtd)
                    {
                        itemPickBin.Quantity = tmpQtd;
                        tmpQtd = 0;
                    }
                    else if (bin.OnHandQty < tmpQtd)
                    {
                        itemPickBin.Quantity = bin.OnHandQty;
                        tmpQtd -= bin.OnHandQty;
                    }

                    listBins.Add(itemPickBin);

                    if (tmpQtd == 0)
                        break;
                }

                if (tmpQtd > 0)
                {
                    await _pickingSLService.UpdatePickingStatusReplenishAsync(OrderStatusEnum.Replenish.ToString(), order.DocEntry, item.ItemCode);
                    throw new ArgumentException("Estoque insuficiente para atender ao pedido");
                }
                   

                item.BinAllocations = listBins;
            }
        }

        public async Task FinishPickingAsync(Picking picking)
        {
            JsonSerializerOptions options = new()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            _publisher.Publish(JsonSerializer.Serialize(picking, options), 
                _configuration.GetSection("Rabbitmq:Invoice.Create:RouteKey").Value, 
                _configuration.GetSection("Rabbitmq:Invoice.Create:ExchangeName").Value);

            var order = await _pickingSLService.GetPickingLoginAsync(picking.DocEntry);
            var activity = await _activityRepository.GetActivityAsync(order.U_CT_LoginWms, picking.DocEntry.ToString());

            if (!activity.Any())
            {
                CultureInfo pt = new CultureInfo("pt-BR");
                Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR", false);
                var dateAjusted = DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss");
                
                await _activityRepository.SaveActivity(new Activity(dateAjusted, order.U_CT_LoginWms, WmsAction.FinishPicking.ToString(), picking.DocEntry.ToString(), picking.BPLId.ToString())); ;
            }

            await _pickingSLService.UpdatePickingStatusByCheckoutAsync(OrderStatusEnum.CanCheckout.ToString(), picking.DocEntry);

        }

        public async Task SavePickingAsync(Picking picking)
        {
            long pickEntry = await _pickingSLService.GetDocEntryPickSavedAsync(picking.DocEntry);

            if (pickEntry == 0)
                await _pickingSLService.CreatePickSavedAsync(picking);
            else
            {
                await _pickingSLService.RemovePickSavedAsync(pickEntry);
                await _pickingSLService.CreatePickSavedAsync(picking);
            }

            await _pickingSLService.UpdatePickingStatusByCheckoutAsync(OrderStatusEnum.SavePicking.ToString(), picking.DocEntry);
        }

        public async Task<IEnumerable<BinLocations>?> SuggestNextBinPickingAsync(string itemCode, string binCode, int lineNum, double remainingQuantity)
        {
            var bins = await _pickingSLService.SuggestNextBinPickingAsync(itemCode, binCode);

            if (bins == null || !bins.ItemsBins.Any())
                return null;

            var listBins = new List<BinLocations>();
            foreach (var item in bins.ItemsBins)
            {
                var tmpQtd = remainingQuantity;

                foreach (var bin in bins.ItemsBins.Where(p => p.ItemCode == item.ItemCode).OrderBy(p => p.BinCode))
                {
                    item.ManBtchNum = bin.ManBtchNum;
                    item.ManSerNum = bin.ManSerNum;

                    var itemPickBin = new BinLocations(lineNum, -1, bin.BinCode, 0, bin.AbsEntry);

                    if (bin.OnHandQty >= tmpQtd)
                    {
                        itemPickBin.Quantity = tmpQtd;
                        tmpQtd = 0;
                    }
                    else if (bin.OnHandQty < tmpQtd)
                    {
                        itemPickBin.Quantity = bin.OnHandQty;
                        tmpQtd -= bin.OnHandQty;
                    }

                    listBins.Add(itemPickBin);

                    if (tmpQtd == 0)
                        break;
                }

                if (tmpQtd > 0)
                    throw new ArgumentException("Estoque insuficiente para atender ao pedido");
            }

            return listBins;
        }
    }
}