using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.ViewModel;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Intelipost;
using Domain.Entities.Orders;
using Domain.Enums;
using Domain.Repositories;
using Domain.Services;
using Infra.Intelipost.Interfaces;
using Infra.ServiceLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftWindowsTCPIP;
using Microsoft.Extensions.Logging.Abstractions;

namespace Api.Controllers
{
    [AllowAnonymous]
    [Route("[controller]")]
    public class OrdersController : Controller
    {
        private readonly ILogger<InventoryController> _logger;
        private readonly IOrdersService _ordersService;
        private readonly IOrdersSLService _sLservice;
        private readonly IApiIntelipost _apiIntelipost;
        private readonly IActivityRepository _activityRepository;
        private readonly IMapper _mapper;

        public OrdersController(ILogger<InventoryController> logger, IOrdersService ordersService, IOrdersSLService sLservice, IApiIntelipost apiIntelipost, IActivityRepository activityRepository, IMapper mapper)
        {
            _logger = logger;
            _ordersService = ordersService;
            _sLservice = sLservice;
            _apiIntelipost = apiIntelipost;
            _activityRepository = activityRepository;
            _mapper = mapper;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("sales-order/brand/{brandId}")]
        public async Task<ActionResult<IEnumerable<SalesOrdersTracking>>> GetOrderSummaryForBillingAsync(int brandId)
        {
            try
            {
                return Ok(await _ordersService.SalesOrdersTrackingAsync(brandId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("sales-order/stores/{branchId}")]
        public async Task<ActionResult<IEnumerable<SalesOrdersTracking>>> GetOrderSummaryForBillingByStoreAsync(int branchId)
        {
            try
            {
                return Ok(await _ordersService.SalesOrdersTrackingByStoreAsync(branchId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("invoice-by-key-acess/{keyAccess}")]
        public async Task<ActionResult<IEnumerable<InvoiceSummaryViewModel>>> GetInvoiceSummaryRequestAsync(string keyAccess)
        {
            try
            {
                var invoice = await _sLservice.GetInvoiceSummaryAsync(keyAccess);

                return Ok(_mapper.Map<InvoiceSummaryViewModel>(invoice?.value.FirstOrDefault()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }



        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("sales-order-by-uf")]
        public async Task<ActionResult<IEnumerable<SalesOrdersTracking>>> GetOrderSummaryForBillingByUfAsync()
        {
            try
            {
                return Ok(await _ordersService.SalesOrdersByUfTrackingAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("dashboard-productivity/bplid/{bplid}/date/{startAt}/action-user/{actionUser}")]
        public async Task<ActionResult> GetDashboardAsync(int bplid, DateTime startAt, string actionUser)
        {
            try
            {
                var activities = await _activityRepository.GetAllActivities(bplid, startAt, actionUser);

                var activity = new DataProdutivity
                {
                    username = "TT",
                    T0 = activities.ToList().Sum(T => T.T0),
                    T1 = activities.ToList().Sum(T => T.T1),
                    T2 = activities.ToList().Sum(T => T.T2),
                    T3 = activities.ToList().Sum(T => T.T3),
                    T4 = activities.ToList().Sum(T => T.T4),
                    T5 = activities.ToList().Sum(T => T.T5),
                    T6 = activities.ToList().Sum(T => T.T6),
                    T7 = activities.ToList().Sum(T => T.T7),
                    T8 = activities.ToList().Sum(T => T.T8),
                    T9 = activities.ToList().Sum(T => T.T9),
                    T10 = activities.ToList().Sum(T => T.T10),
                    T11 = activities.ToList().Sum(T => T.T11),
                    T12 = activities.ToList().Sum(T => T.T12),
                    T13 = activities.ToList().Sum(T => T.T13),
                    T14 = activities.ToList().Sum(T => T.T14),
                    T15 = activities.ToList().Sum(T => T.T15),
                    T16 = activities.ToList().Sum(T => T.T16),
                    T17 = activities.ToList().Sum(T => T.T17),
                    T18 = activities.ToList().Sum(T => T.T18),
                    T19 = activities.ToList().Sum(T => T.T19),
                    T20 = activities.ToList().Sum(T => T.T20),
                    T21 = activities.ToList().Sum(T => T.T21),
                    T22 = activities.ToList().Sum(T => T.T22),
                    T23 = activities.ToList().Sum(T => T.T23),
                    TT = activities.ToList().Sum(T => T.TT)
                };

                var report = activities.Append(activity).ToList();
                
                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("summary-details/range/{range}/status/{status}/branch/{branchId}")]
        public async Task<ActionResult<IEnumerable<OrderDetails>>> GetOrderSummaryDetails(string range, string status, int branchId)
        {
            try
            {
                return Ok(await _ordersService.SalesOrdersDetailsAsync(range, status, branchId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("summary/bplId/{bplId}/store/{store}/status/{status}")]
        public async Task<ActionResult<IEnumerable<OrderDetails>>> GetOrderSummaryDetailsByStore(int bplId, string store, string status)
        {
            try
            {
                var obj = await _sLservice.GetDetailsSummarySalesOrderByStore(bplId, status, store);
                return Ok(obj?.value);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
 

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("orders/docEntry/{docEntry}")]
        public async Task<ActionResult<IEnumerable<OrderDetails>>> GetOrderSummaryDetails(long docEntry)
        {
            try
            {
                return Ok(await _sLservice.GetOrdersByDocEntry(docEntry.ToString()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("orders/danfe/docEntry/{docEntry}")]
        public async Task<ActionResult<IEnumerable<OrderDetails>>> GetLinkDanfe(long docEntry)
        {
            try
            {
                return Ok(await _sLservice.GetKeyAccessByDocEntryOrder(docEntry));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("late-orders")]
        public async Task<ActionResult> GetLateOrders()
        {
            try
            {
                var lateOrders = await _sLservice.GetLateOrders();
                return Ok(lateOrders.LateOrder);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost("search-orders-by-reference")]
        public async Task<ActionResult<IEnumerable<OrderDetails>>> GetOrderSummaryDetails([FromBody] Reference reference)
        {
            try
            {
                return Ok(await _ordersService.SalesOrdersByClientAsync(reference.body));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost("set-tracking-intelipost")]
        public async Task<ActionResult> SetTrackingByOrderIntelipost([FromBody] List<TrackingUpdate> orders)
        {
            try
            {
                foreach(var o in orders)
                {
                    var tracking = new TrackingData();

                    tracking.order_number = o.order;
                    tracking.tracking_data_array = new List<TrackingDataArray>();
                    tracking.tracking_data_array.Add(new TrackingDataArray
                    {
                        shipment_order_volume_number = 1,
                        tracking_code = o.tracking
                    });

                    await _apiIntelipost.SetTrackingOrderOnIntelipost(tracking);
                }
             
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpPost("cancel/{docEntry}")]
        public async Task<IActionResult> CancelOrderSAP(long docEntry)
        {
            try
            {
                await _ordersService.CancelOrderByDocEntryAsync(docEntry);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("orders-open-marketplace")]
        public async Task<IActionResult> RetriveOrdersMarketPlace()
        {
            try
            {
                var order = await _sLservice.GetOpenMarketPlaceOrders();
                return Ok(order?.value);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPatch("docEntry/{docEntry}/group/{group}/status/{status}/comments/{comments}")]
        public async Task<IActionResult> UpdateOrdersMarketPacle(long docEntry, string group, string status, string comments)
        {
            try
            {
                await _sLservice.UpdateOrder(docEntry, group, status, comments);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}

