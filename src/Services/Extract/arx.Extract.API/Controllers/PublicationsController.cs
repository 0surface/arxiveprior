using arx.Extract.API.Services;
using arx.Extract.Data.Repository;
using arx.Extract.Types;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace arx.Extract.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PublicationsController : Controller
    {        
        private readonly IPublicationRepository _repo;
        private readonly IMapper _mapper;

        public PublicationsController(IPublicationRepository repo, IMapper mapper)
        {     
            _repo = repo;
            _mapper = mapper;
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

            var entities = await _repo.GetSubjectInclusiveBetweenDates(subjectCode, updatedFromDate, updatedToDate);

            var result = (entities.Count > 0) ? _mapper.Map<List<PublicationItem>>(entities) : null;           

            if (result == null)
                return StatusCode((int)HttpStatusCode.InternalServerError);

            if (result.Count == 0)
                return NoContent();

            return Json(result);
        }

        [HttpGet("fulfillmentid")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetPublicationsByFulfilmentId(string fulfilmentId)
        {
            if(string.IsNullOrEmpty(fulfilmentId))
                return StatusCode((int)HttpStatusCode.BadRequest);

            var entities = await _repo.GetByFulfilmentId(fulfilmentId);

            var result = (entities.Count > 0) ? _mapper.Map<List<PublicationItem>>(entities) : null;

            if (result == null)
                return StatusCode((int)HttpStatusCode.InternalServerError);

            if (result.Count == 0)
                return NoContent();

            return Json(result);
        }
    }
}
