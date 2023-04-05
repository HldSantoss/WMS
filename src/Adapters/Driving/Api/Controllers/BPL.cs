using Api.ViewModel;
using Domain.Entities;
using Infra.ServiceLayer.Interfaces;
using Infra.ServiceLayer.Operations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]")]
    public class BPL : BaseController
    {
        private readonly IBranchesSLService _branchesSLService;
        private readonly ILogger<InventoryController> _logger;

        public BPL(IBranchesSLService branchesSLService, ILogger<InventoryController> logger)
        {
            _branchesSLService = branchesSLService;
            _logger = logger;
        }

        [HttpGet("all-branches")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Branches>>> GetInventoryBalanceBinLocationsByItemCodeAsync()
        {
            try
            {
                return Ok(await _branchesSLService.GetAllBranch());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
