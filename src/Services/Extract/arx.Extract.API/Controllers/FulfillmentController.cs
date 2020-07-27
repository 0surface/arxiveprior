using arx.Extract.API.Services;
using arx.Extract.Types;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace arx.Extract.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class FulfillmentController : Controller
    {
        private readonly IFulfillmentService _fulfillmentService;

        public FulfillmentController(IFulfillmentService fulfillmentService)
        {
            _fulfillmentService = fulfillmentService;
        }

        [HttpGet()]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetFulfillments(string jobName)
        {
            if (string.IsNullOrEmpty(jobName))
                return StatusCode((int)HttpStatusCode.BadRequest);

            var result = await _fulfillmentService.GetFulfillments(jobName);

            if (result == null)
                return StatusCode((int)HttpStatusCode.InternalServerError);

            if (result.Count == 0)
                return NoContent();

            return Json(result);
        }

        [HttpGet("lastsuccess")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetLastFulfillment(string jobName)
        {
            if (string.IsNullOrEmpty(jobName))
                return StatusCode((int)HttpStatusCode.BadRequest);

            var result = await _fulfillmentService.GetLastSuccessfulFulfillment(jobName);

            if (result == null)
                return StatusCode((int)HttpStatusCode.InternalServerError);

            if (result == new Fulfillment())
                return NoContent();

            return Json(result);
        }

        [HttpGet("querydates")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetFulfillmentsBetweenQueryDates(string jobName, DateTime queryFromDate, DateTime queryToDate)
        {
            if (string.IsNullOrEmpty(jobName)
                || queryFromDate == null || queryFromDate == DateTime.MinValue
                || queryToDate == null || queryToDate == DateTime.MinValue
                || queryFromDate > queryToDate)
                return StatusCode((int)HttpStatusCode.BadRequest);

            var result = await _fulfillmentService.GetFulfillmentsBetweenQueryDates(jobName, queryFromDate, queryToDate);

            if (result == null)
                return StatusCode((int)HttpStatusCode.InternalServerError);

            if (result.Count == 0)
                return NoContent();

            return Json(result);
        }

        [HttpGet("failed")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetFailedFulfillmenta(string jobName)
        {
            if (string.IsNullOrEmpty(jobName))
                return StatusCode((int)HttpStatusCode.BadRequest);

            var result = await _fulfillmentService.GetFailedFulfillments(jobName);

            if (result == null)
                return StatusCode((int)HttpStatusCode.InternalServerError);

            if (result.Count == 0)
                return NoContent();

            return Json(result);
        }
    }
}
