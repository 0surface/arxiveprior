using AutoMapper;
using Common.Util.ViewModel;
using Journal.API.Application.Models.Subject;
using Journal.API.Services;
using Journal.Domain.AggregatesModel.SubjectAggregate;
using Journal.Infrastructure;
using Journal.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Journal.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]    
    public class SubjectsController : Controller
    {
        private readonly ISubjectRepository _subjectRepository;
        private readonly IMapper _mapper;

        public SubjectsController(JournalContext journalContext, IMapper mapper)
        {
            _subjectRepository = new SubjectRepository(journalContext);
            _mapper = mapper;
        }

        [HttpGet]
        [Route("items")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(List<SubjectItem>),(int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> GetAll()
        {
            var result =  await _subjectRepository.GetAllSubjects();

            if (result == null)
                return StatusCode((int)HttpStatusCode.InternalServerError);

            if (result.Count() == 0)
                return NoContent();

            return Json(_mapper.Map<List<SubjectItem>>(result));
        }

        [HttpGet]
        [Route("items/group/{groupCode:minlength(2)}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<SubjectItem>), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> GetByGroup(string groupCode)
        {
            if (string.IsNullOrEmpty(groupCode) || groupCode.Length < 2)
                return BadRequest();

            var result = await _subjectRepository.GetSubjectsByGroupCode(groupCode);

            if (result == null)
                return StatusCode((int)HttpStatusCode.InternalServerError);

            if (result.Count() == 0)
                return NoContent();

            return Json(_mapper.Map<List<SubjectItem>>(result));
        }

        [HttpGet]
        [Route("items/discipline/{discipline:minlength(7)}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(List<SubjectItem>), (int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> GetByDiscipline(string discipline)
        {
            if (string.IsNullOrEmpty(discipline) || discipline.Length < 10)
                return BadRequest();

            var result = await _subjectRepository.GetSubjectsByDiscipline(discipline);

            if (result == null)
                return StatusCode((int)HttpStatusCode.InternalServerError);

            if (result.Count() == 0)
                return NoContent();

            return Json(_mapper.Map<List<SubjectItem>>(result));
        }
    }
}
