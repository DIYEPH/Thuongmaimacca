using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TMDT_MOHINHMACCA.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Bank")]
    [Authorize(Roles = "1")]
    public class BankController : Controller
    {
        [Route("Index")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
