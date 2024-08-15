using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMDT_MOHINHMACCA.Models;

namespace TMDT_MOHINHMACCA.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Post")]
    [Authorize(Roles = "1")]
    public class PostController : Controller
    {
        private readonly ShopmaccaContext _db;
        public PostController(ShopmaccaContext db)
        {
            _db = db;
        }
        [Route("Index")]
        public IActionResult Index()
        {
            var posts = _db.Posts.Include(p=>p.Cate).Where(p=> p.Status!="0" && p.Status!="c").ToList();
            return View(posts);
        }
    }
}
