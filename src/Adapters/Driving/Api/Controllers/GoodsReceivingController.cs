using AutoMapper;
using Domain.Entities;
using Domain.Entities.ReceiptOfGoods;
using Domain.Enums;
using Domain.Repositories;
using Domain.Services;
using Infra.ServiceLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Api.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class GoodsReceivingController : BaseController
    {
        private readonly IGoodsReceivingSLService _goodsReceivingSLService;
        private readonly IActivityRepository _activityRepository;
        private readonly IActivityLogSLService _activityLogSLService;
        private readonly IGoodsReceivingService _goodsReceivingService;
        private readonly IMapper _mapper;

        public GoodsReceivingController(IGoodsReceivingSLService goodsReceivingSLService, IActivityRepository activityRepository, IActivityLogSLService activityLogSLService, IGoodsReceivingService goodsReceivingService, IMapper mapper)
        {
            _goodsReceivingSLService = goodsReceivingSLService;
            _activityRepository = activityRepository;
            _activityLogSLService = activityLogSLService;
            _goodsReceivingService = goodsReceivingService;
            _mapper = mapper;
        }

        [HttpPost("start-receiving/{keyaccess}/user/{userName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> StartReceivingAsync(string keyaccess, string userName)
        {
            try
            {
                return Ok(await _goodsReceivingService.StartReceivingAsync(keyaccess,userName));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("receiving-item-by-sky/{docEntry}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> ReceivingMaterialBySku([FromBody] ReceivingItemWms item, long docEntry)
        {
            try
            {
                await _goodsReceivingService.ReceivingItemBySkuAsync(item, docEntry);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("receiving-item-by-series/{docEntry}/line-num/{lineNum}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ReceivingMaterialBySeries([FromBody] ReceivingItemBySerie item, long docEntry, int lineNum)
        {
            try
            {
                await _goodsReceivingService.ReceivingItemBySerieAsync(item, docEntry, lineNum);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("scheduling-receiving/inicial/{startAt}/final/{finishAt}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> SchedulingReceiving(DateTime startAt, DateTime finishAt)
        {
            try
            {
                var obj = await _goodsReceivingSLService.GetSchedulingPurchaseOrderByDateAsync(startAt, finishAt);
                return Ok(obj?.value);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("scheduling-receiving/details/doc-entry/{docEntry}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> SchedulingReceivingDetailsByDocEntryAsync(long docEntry)
        {
            try
            {
                return Ok(await _goodsReceivingSLService.GetSeriesItemAsync(docEntry));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("undo-item/{docEntry}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UndoItem([FromBody] Item item, long docEntry)
        {
            try
            {
                await _goodsReceivingService.UndoItemAsync(item, docEntry);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("finish-receiving/{keyaccess}/user/{userName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> FinishReceiving(string keyaccess, long docEntry, string userName)
        {
            try
            {
                await _goodsReceivingService.FinishReceivingAsync(keyaccess, docEntry, userName);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("search-testing-by-key-access/{keyAccess}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SearchTesting(string keyAccess)
        {
            try
            { 
                return Ok(await _goodsReceivingService.DanfeEquipamentAsync(keyAccess));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("get-equipament-with-testing-by-serie/serie/{serie}/user/{user}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetEquipamentWithTesting(string serie, string user)
        {
            try
            {
                var series = await _goodsReceivingSLService.GetSerialNumbersObjBySerieAsync(serie);


                if (series?.Value.Count() == 0)
                {
                    return NotFound();
                }


                CultureInfo pt = new CultureInfo("pt-BR");
                Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR", false);
                var dateAjusted = DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss");

                await _activityRepository.SaveActivity(new Activity(dateAjusted, user, WmsAction.StartTesting.ToString(), serie, "1")); ; 


                return Ok(series?.Value);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("update-testing/serie/{serie}/docEntry/{docEntry}/user/{userName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateItemTesting([FromBody] Preparation preparation, string serie, long docEntry, string userName)
        {
            try
            {
                await _goodsReceivingSLService.UpdateSerialNumbersObjBySerieAsync(preparation, docEntry);

              
                CultureInfo pt = new CultureInfo("pt-BR");
                Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR", false);
                var dateAjusted = DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss");

                await _activityRepository.SaveActivity(new Activity(dateAjusted, userName, WmsAction.FinishTesting.ToString(), serie, "1"));

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}