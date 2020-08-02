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
        private readonly IArticlesService _articlesService;

        public ArticlesController(IArticlesService articlesService)
        {
            _articlesService = articlesService;
        }

        
    }
}
