using Journal.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace Journal.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ArticlesController : Controller
    {
        private readonly IArticleService _articlesService;

        public ArticlesController(IArticleService articlesService)
        {
            _articlesService = articlesService;
        }

        [HttpGet()]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetArticles(string fulfillmentId)
        {
            if (string.IsNullOrEmpty(fulfillmentId))
                return StatusCode((int)HttpStatusCode.BadRequest);

            var result = await _articlesService.GetArticles(fulfillmentId);

            if (result == null || result.PublicationItems == null)
                return StatusCode((int)HttpStatusCode.InternalServerError);

            if (result.PublicationItems.Count == 0)
                return NoContent();

            return Json(result);
        }

    }
}
