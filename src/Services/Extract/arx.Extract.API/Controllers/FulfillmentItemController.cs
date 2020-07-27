using arx.Extract.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace arx.Extract.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class FulfillmentItemController : Controller
    {
        private readonly IFulfillmentItemService _service;

        public FulfillmentItemController(IFulfillmentItemService service)
        {
            _service = service;
        }

        [HttpGet()]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetItems(string fulfillmentId)
        {
            if (string.IsNullOrEmpty(fulfillmentId))
                return StatusCode((int)HttpStatusCode.BadRequest);

            var result = await _service.GetItems(fulfillmentId);
            if (result == null)
                return StatusCode((int)HttpStatusCode.InternalServerError);

            if (result.Count == 0)
                return NoContent();

            return Json(result);
        }
    }
}
