using Api.ViewModel;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Inventories;
using Domain.Services;
using Infra.ServiceLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]")]
    public class InventoryController : BaseController
    {
        private readonly ILogger<InventoryController> _logger;
        private readonly IInventorySLService _inventorySLService;
        private readonly ITransferService _transferService;
        private readonly IMapper _mapper;

        public InventoryController(ILogger<InventoryController> logger,
                                   IInventorySLService inventorySLService,
                                   ITransferService transferService,
                                   IMapper mapper)
        {
            _logger = logger;
            _inventorySLService = inventorySLService;
            _transferService = transferService;
            _mapper = mapper;
        }

        [HttpGet("balance-bin-locations-by-itemcode/{itemCode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<InventoryViewModel>>> GetInventoryBalanceBinLocationsByItemCodeAsync(string itemCode)
        {
            var userName = GetUserName();

            if (string.IsNullOrWhiteSpace(itemCode))
                return BadRequest("itemCode é necessário.");

            var inventories = await _inventorySLService.InventoryBalanceBinLocationsByItemCodeAsync(itemCode);

            if (inventories == null || !inventories.Value.Any())
                return NotFound();

            try
            {
                return Ok(_mapper.Map<List<InventoryViewModel>>(inventories.Value));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("balance-bin-locations-by-serie/{serie}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetInventoryBalanceBinLocationsBySerieAsync(string serie)
        {
            if (string.IsNullOrWhiteSpace(serie))
                return BadRequest("série é necessária.");

            var inventories = await _inventorySLService.InventoryBalanceBinLocationsBySerieAsync(serie);

            if (inventories == null || !inventories.Value.Any())
                return NotFound();

            try
            {
                return Ok(_mapper.Map<List<InventoryViewModel>>(inventories.Value));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("balance-bin-locations-by-bin-code/{binCode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetInventoryBinLocationsByBinCodeAsync(string binCode)
        {
            if (string.IsNullOrWhiteSpace(binCode))
                return BadRequest("bin code é necessário.");

            var inventories = await _inventorySLService.InventoryBalanceBinLocationsByBinCodeAsync(binCode);

            if (inventories == null || !inventories.Value.Any())
                return NotFound();

            try
            {
                return Ok(_mapper.Map<List<InventoryViewModel>>(inventories.Value));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("series-by-bin-code/{binCode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetInventoryBinLocationsBySerieAsync(string binCode)
        {
            if (string.IsNullOrWhiteSpace(binCode))
                return BadRequest("bin code é necessário.");

            var inventories = await _inventorySLService.InventoryBalanceSeriesByBinCodeAsync(binCode);

            if (inventories == null || !inventories.Value.Any())
                return NotFound();

            try
            {
                return Ok(_mapper.Map<List<InventoryViewModel>>(inventories.Value));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("stock-virtual")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetInventoryStockVirtualAsync()
        {
            var inventories = await _inventorySLService.GetStockVirtualAsync();

            if (inventories == null || !inventories.value.Any())
                return NotFound();

            try
            {
                return Ok(inventories.value);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("stock-virtual/item-code/{itemCode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetInventoryStockVirtualByItemCodeAsync(string itemCode)
        {
            if (string.IsNullOrWhiteSpace(itemCode))
                return BadRequest("itemCode é necessário.");

            var inventories = await _inventorySLService.GetStockVirtualByItemCodeAsync(itemCode);

            if (inventories == null || !inventories.value.Any())
                return NotFound();

            try
            {
                return Ok(inventories.value);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("stock-virtual/docEntry/{docEntry}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetInventoryStockVirtualByDocEntryAsync(string docEntry)
        {
            if (string.IsNullOrWhiteSpace(docEntry))
                return BadRequest("docEntry é necessário.");

            var inventories = await _inventorySLService.GetStockVirtualByDocEntryAsync(docEntry);

            if (inventories == null || !inventories.value.Any())
                return NotFound();

            try
            {
                return Ok(inventories.value);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("stock-virtual/remove/{docEntry}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteStockVirtualByDocEntryAsync(string docEntry)
        {
            if (string.IsNullOrWhiteSpace(docEntry))
                return BadRequest("docEntry é necessário.");

            try
            {
                await _inventorySLService.DeleteStockVirtualAsync(docEntry);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("stock-virtual")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateStockVirtualByDocEntryAsync(UpdateObj obj)
        {
            if (string.IsNullOrWhiteSpace(obj.U_ItemCode))
                return BadRequest("ItemCode é necessário.");

            try
            {
                await _inventorySLService.PostStockVirtualAsync(obj);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("stock-virtual")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateStockVirtualByDocEntryAsync(UpdateObj obj)
        {
            if (string.IsNullOrWhiteSpace(obj.U_ItemCode))
                return BadRequest("ItemCode é necessário.");

            try
            {
                await _inventorySLService.UpdateStockVirtualAsync(obj);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("stock-by-warehouse/{itemCode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateStockVirtualByDocEntryAsync(string  itemCode)
        {
            if (string.IsNullOrWhiteSpace(itemCode))
                return BadRequest("ItemCode é necessário.");

            try
            {
                var stock = await _inventorySLService.GetStockAllWarehouseAvaliable(itemCode);

                return Ok(stock.value);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("balance-bin-locations-by-gtin/{gtin}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetInventoryBinLocationsByGtinAsync(string gtin)
        {
            if (string.IsNullOrWhiteSpace(gtin))
                return BadRequest("gtin é necessário.");

            var inventories = await _transferService.InventoryBalanceBinLocationsByGtinServiceAsync(gtin);

            if (inventories == null || !inventories.Value.Any())
                return NotFound();

            try
            {
                return Ok(_mapper.Map<List<InventoryViewModel>>(inventories.Value));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("transfer-item-bin-location-by-gtin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> TransferItemByGtinAsync(TransferByGtinViewModel transferViewModel)
        {
            try
            {
                var transfer = await _transferService.TranferItemByGtinAsync(transferViewModel.Gtin, 
                                            transferViewModel.Quantity, 
                                            transferViewModel.BinCodeFrom, 
                                            transferViewModel.BinCodeTo);

                if (transfer == false)
                    return NotFound();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return NoContent();
        }

        [HttpPost("transfer-item-bin-location-by-serie")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> TransferItemBySerieAsync(TransferBySerieViewModel transferViewModel)
        {
            try
            {
                var transfer = await _transferService.TranferItemBySerieAsync(transferViewModel.Serie, 
                                            transferViewModel.Quantity, 
                                            transferViewModel.BinCodeFrom, 
                                            transferViewModel.BinCodeTo);

                if (transfer == false)
                    return NotFound();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return NoContent();
        }

        [HttpPost("transfer-item-bin-location-by-itemcode")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> TransferItemByItemCodeAsync(TransferByItemCodeViewModel transferViewModel)
        {
            try
            {
                var transfer = await _transferService.TranferItemByItemCodeAsync(transferViewModel.ItemCode, 
                                            transferViewModel.Quantity, 
                                            transferViewModel.BinCodeFrom, 
                                            transferViewModel.BinCodeTo);

                if (transfer == false)
                    return NotFound();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return NoContent();
        }

    }
}