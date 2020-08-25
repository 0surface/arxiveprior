using Journal.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Journal.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ArticlesController : Controller
    {
        public ArticlesController(IArticlesService articlesService)
        {

        }
    }
}
