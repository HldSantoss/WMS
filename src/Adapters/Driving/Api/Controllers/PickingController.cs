using Api.ViewModel;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using Domain.Services;
using Infra.ServiceLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]")]
    public class PickingController : BaseController
    {
        private readonly IPickingService _pickingService;
        private readonly IMapper _mapper;
        private readonly IPickingSLService _pickingSLService;
        private readonly IInventorySLService _inventorySLService;
        private readonly IActivityRepository _activityRepository;

        public PickingController(IPickingService pickingService, IMapper mapper, IPickingSLService pickingSLService, IInventorySLService inventorySLService, IActivityRepository activityRepository)
        {
            _pickingService = pickingService;
            _mapper = mapper;
            _pickingSLService = pickingSLService;
            _inventorySLService = inventorySLService;
            _activityRepository = activityRepository;
        }

        [HttpGet("start/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<PickingViewModel>>> StartAsync(string userId)
        {
            try
            {
                var picking = await _pickingService.GetPicking(userId);

                return Ok(_mapper.Map<PickingViewModel>(picking));
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message});
            }
        }

        [HttpGet("continue/{docEntry}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PickingViewModel>>> ContinueAsync(long docEntry)
        {
            try
            {
                var picking = await _pickingService.ContinuePicking(docEntry);

                if (picking == null)
                    return NotFound();

                return Ok(_mapper.Map<PickingViewModel>(picking));
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message});
            }
        }

        [HttpPost("finish")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> FinishPickingAsync(PickingViewModel picking)
        {
            if (!ModelState.IsValid)
                return BadRequest(new {error = string.Join(", ", ModelState.Select(p => p.Value))});

            if (picking.Items.Where(p => p.IsFinish == false).Any())
                return BadRequest(new {error = "Alguns itens não foram coletados"});
        
            try
            {
                await _pickingService.FinishPickingAsync(_mapper.Map<Picking>(picking));

                return Accepted();
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message});
            }
        }

        [HttpPut("save")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> SavePickingAsync(PickingViewModel picking)
        {
            try
            {
                await _pickingService.SavePickingAsync(_mapper.Map<Picking>(picking));

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message});
            }
        }

        [HttpGet("suggest-next-bin/{itemCode}/{binCode}/{lineNum}/{remainingQuantity}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<BinLocations>>> SuggestNextBinPickingAsync(string itemCode, string binCode, int lineNum, double remainingQuantity)
        {
            try
            {
                var bins = await _pickingService.SuggestNextBinPickingAsync(itemCode, binCode, lineNum, remainingQuantity);

                if (bins == null)
                    return NotFound();

                return Ok(_mapper.Map<IEnumerable<BinLocations>>(bins));
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message});
            }
        }
    
        [HttpGet("itemcode-by-serie/{serie}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> ItemCodeBySerie(string serie)
        {
            try
            {
                var itemCode = await _inventorySLService.ItemCodeBySerieAsync(serie);

                if (itemCode == null)
                    return NotFound();

                return Ok(new { ItemCode = itemCode });
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message});
            }
        }

        [HttpGet("itemcode-by-serie-and-bincode/{serie}/{bincode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> ItemCodeBySerie(string serie, string bincode)
        {
            try
            {
                var itemCode = await _inventorySLService.ItemCodeBySerieAsync(serie, bincode);

                if (itemCode == null)
                    return NotFound();

                return Ok(new { ItemCode = itemCode });
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message});
            }
        }

        [HttpGet("errors")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> GetErrorsIntegration()
        {
            try
            {
                var result = await _pickingSLService.GetErrorDetails();
                return Ok(result?.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}