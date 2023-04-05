using Api.ViewModel;
using AutoMapper;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]")]
    public class CheckoutController : BaseController
    {
        private readonly ILogger<InventoryController> _logger;
        private readonly ICheckoutService _checkoutService;
        private readonly IMapper _mapper;

        public CheckoutController(ILogger<InventoryController> logger,
                                  ICheckoutService checkoutService,
                                  IMapper mapper)
        {
            _logger = logger;
            _checkoutService = checkoutService;
            _mapper = mapper;
        }

        [HttpGet("start/{orderEntry}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PickingViewModel>> StartCheckout(long orderEntry, [FromQuery] string? userName)
        {
            if (orderEntry <= 0)
                return BadRequest("orderEntry é necessário.");

            try
            {
                var order = await _checkoutService.GetOrderAsync(orderEntry, userName);

                if (order == null)
                    return NotFound();

                return Ok(_mapper.Map<PickingViewModel>(order));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("finish/{orderEntry}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PickingViewModel>> FinishCheckout(long orderEntry, [FromQuery] string? userName)
        {
            if (orderEntry <= 0)
                return BadRequest("orderEntry é necessário.");

            try
            {
                await _checkoutService.FinishAsync(orderEntry, userName);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}