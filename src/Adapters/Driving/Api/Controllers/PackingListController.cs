using System.Net.Mime;
using System.Net.NetworkInformation;
using Api.ViewModel;
using AutoMapper;
using Domain.Entities;
using Domain.Services;
using Infra.ServiceLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace Api.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]")]
    public class PackingListController : BaseController
    {
        private readonly IPackingListService _packingListService;
        private readonly IPackingListSLService _packingListSLService;
        private readonly IMapper _mapper;

        public PackingListController(IPackingListService packingListService, IPackingListSLService packingListSLService, IMapper mapper)
        {
            _packingListService = packingListService;
            _packingListSLService = packingListSLService;
            _mapper = mapper;
        }


        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PackingListViewModel>> CreatePackingListAsync(PackingListCreateViewModel packingListCreate)
        {
            try
            {
                var packingList = await _packingListSLService.CreatePackingListAsync(packingListCreate.CarrierId, packingListCreate.Method, packingListCreate.Branch);

                return Created("",_mapper.Map<PackingListViewModel>(packingList));
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message});
            }
        }

        [HttpGet("startAt/{startAt}/finishAt/{finishAt}/status/{status}/bplId/{bplId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PackingList>>> GetAllPackingListAsync(DateTime startAt, DateTime finishAt, string status, string bplId)
        {
            try
            {
                var packingList = await _packingListSLService.GetAllPackingListAsync(startAt, finishAt, status, bplId);
                var allPackingList = packingList?.Packinglists.OrderBy(p => p.U_CarrierId).ToList();

                return Ok(allPackingList);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{docEntry}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PackingListViewModel>> GetPackingListAsync(long docEntry)
        {
            try
            {
                var packingList = await _packingListSLService.GetPackingListAsync(docEntry);

                if (packingList == null)
                    return NotFound();

                return Ok(_mapper.Map<PackingListViewModel>(packingList));
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message});
            }
        }

        [HttpDelete("{docEntry}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> RemovePackingList(long docEntry)
        {
            try
            {
                var packingList = await _packingListService.RemovePackingListAsync(docEntry);

                if (packingList == null)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message});
            }
        }

        [HttpPost("close-packing-list/{docEntry}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ClosePackingList(long docEntry)
        {
            try
            {
                var packingList = await _packingListService.ClosePackingListAsync(docEntry);

                if (packingList == null)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message});
            }
        }

        [HttpPatch("add-item/{docEntry}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddItemPackingList(long packingListEntry, string keyNfe)
        {
            try
            {
                if (packingListEntry <= 0)
                    throw new ArgumentException("docEntry (id da lista de embarque) é obrigatório");

                if (string.IsNullOrWhiteSpace(keyNfe) || keyNfe.Length != 44)
                    throw new ArgumentException("keyNfe inválida");

                await _packingListService.AddItemPackingListAsync(packingListEntry, keyNfe, false);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message});
            }
        }


        [HttpPost("add-item-intelipost/{serial}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddItemIntelipostList(string serial)
        {
             
            try
            {
                await _packingListService.AddItemIntelipostAsync(serial, true);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
                   
        }

        [HttpPatch("add-item/{docEntry}/resend")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddItemPackingListResend(long packingListEntry, string keyNfe)
        {
            try
            {
                if (packingListEntry <= 0)
                    throw new ArgumentException("docEntry (id da lista de embarque) é obrigatório");

                if (string.IsNullOrWhiteSpace(keyNfe) || keyNfe.Length != 44)
                    throw new ArgumentException("keyNfe inválida");

                await _packingListService.AddItemPackingListAsync(packingListEntry, keyNfe, true);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{packingListEntry}/{keyNfe}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> RemoveItemPackingList(long packingListEntry, string keyNfe)
        {
            try
            {
                if (packingListEntry <= 0)
                    throw new ArgumentException("docEntry (id da lista de embarque) é obrigatório");

                if (string.IsNullOrWhiteSpace(keyNfe) || keyNfe.Length != 44)
                    throw new ArgumentException("keyNfe inválida");

                await _packingListService.RemoveItemPackingListAsync(packingListEntry, keyNfe);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new {error = ex.Message});
            }
        }

        [HttpGet("print/{packingListEntry}")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK, MediaTypeNames.Application.Pdf)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Print(long packingListEntry)
        {
            try
            {
                var file = _packingListService.GetPdfPackingList(packingListEntry);

                if (file == null)
                    return NotFound();

                return File(file, "application/pdf", $"packingList_{packingListEntry}.pdf");
            }
            catch (Exception ex)
            {                
                return BadRequest(new {error = ex.Message});
            }
        }

        [HttpPost("shipped/{packingListEntry}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> PostSendOrders(long packingListEntry)
        {
            try
            {
                await _packingListService.SendOrders(packingListEntry);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}