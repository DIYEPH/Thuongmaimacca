using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TMDT_MOHINHMACCA.ViewComponents;

namespace TMDT_MOHINHMACCA.Controllers
{
    [Authorize]
    public class SearchController : Controller
    {

        [HttpGet]
        public IActionResult Search(int? page, int? categoryid, string searchInput, int rangeInput)
        {
            return ViewComponent("PostsShop", new { page, categoryid, searchInput, rangeInput });
        }
    }
}
