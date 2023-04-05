using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class LabelController : ControllerBase
    {
        private readonly IPackingListService _packingListService;

        public LabelController(IPackingListService packingListService)
        {
            _packingListService = packingListService;
        }


        [HttpPost("danfe-simplificada/printer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<string>> GetDanfeSimplificada(string printer, int boxes, string? orderEntry, string? keynfe)
        {
            try
            {
                await _packingListService.PrintDanfe(printer, boxes, orderEntry, keynfe);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
           }
            
        }

        [HttpPost("volumes/{orderEntry}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<string>> GetVolumes(long orderEntry)
        {
            throw new NotImplementedException();
        }
    }
}