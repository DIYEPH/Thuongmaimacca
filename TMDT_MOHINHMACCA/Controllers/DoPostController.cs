using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMDT_MOHINHMACCA.Models;
using TMDT_MOHINHMACCA.Services;

namespace TMDT_MOHINHMACCA.Controllers
{

    public class DoPostController : Controller
    {
        private readonly ShopmaccaContext _db;
        private readonly IFirebaseStorageService _firebaseStorageService;

        public DoPostController(IFirebaseStorageService firebaseStorageService,ShopmaccaContext db)
        {
            _db = db;
            _firebaseStorageService=firebaseStorageService;
        }
        [Authorize]
        [Route("/uppost")]
        [HttpGet]
        public IActionResult Index(int idchoose)
        {
            Post post = new Post();
            return View(post);
        }
        [Route("/choose")]
        public IActionResult Choose()
        {
            var choose = _db.Chooses.AsNoTracking().OrderBy(p => p.ChooseTime).ToList();
            return View(choose);
        }
        [Route("/postdetail")]
        public IActionResult PostDetail(int id)
        {
            var post = _db.Posts.Include(p=>p.Cate).Include(p=>p.UsernameNavigation).Where(p=>p.PostId== id).FirstOrDefault();
            var orderbypost = _db.Orders.Where(p => p.PostId == id && p.Star != null).Include(p=>p.BuyerNavigation).Include(p=>p.OrderDetails).ToList();
            
            ViewBag.ListOrer = orderbypost;
            return View(post);
        }
    }
}
