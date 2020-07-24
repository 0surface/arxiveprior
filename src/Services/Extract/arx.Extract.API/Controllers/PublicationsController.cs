using arx.Extract.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace arx.Extract.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PublicationsController : Controller
    {
        private readonly IPublicationsService _service;

        public PublicationsController(IPublicationsService service)
        {
            _service = service;
        }

        [HttpGet("subjectbetweendates")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetBySubjectCodeByUpdatedDates(string subjectCode, DateTime updatedFromDate, DateTime updatedToDate)
        {
            if (string.IsNullOrWhiteSpace(subjectCode)
                || updatedFromDate == null || updatedFromDate == DateTime.MinValue
                || updatedToDate == null || updatedToDate == DateTime.MinValue
                || updatedFromDate > updatedToDate)
                return StatusCode((int)HttpStatusCode.BadRequest);

            var result = await _service.GetBySubjectCodeByUpdatedDates(subjectCode, updatedFromDate, updatedToDate);

            if (result == null)
                return StatusCode((int)HttpStatusCode.InternalServerError);

            if (result.Count == 0)
                return NoContent();

            return Json(result);
        }
    }
}
