using Journal.API.Services;
using Journal.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Journal.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ArticlesController : Controller
    {
        private readonly ISubjectService _subjectService;

        public ArticlesController(JournalContext journalContext)
        {
            _subjectService = new SubjectService(journalContext);
        }
        //public ArticlesController(SubjectService subjectService)
        //{
        //    _subjectService = subjectService;
        //}

        [HttpGet("subjects")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult GetAction()
        {
            var result = _subjectService.GetAll();

            if (result == null)
                return StatusCode((int)HttpStatusCode.InternalServerError);

            if (result.Count == 0)
                return StatusCode((int)HttpStatusCode.NoContent);

            return Json(result);
        }
    }
}
