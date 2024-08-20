using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMDT_MOHINHMACCA.Models;

namespace TMDT_MOHINHMACCA.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ShopmaccaContext _db;
        public OrderController(ShopmaccaContext db)
        {
            _db = db;
        }
        [HttpGet]
        [Route("/myorders")]
        public IActionResult Orders()
        {
            string username = User.Identity.Name;
            var order = _db.Orders.Include(p => p.Post.UsernameNavigation).Where(p => p.Buyer == username).OrderByDescending(p => p.OrderTime).ToList();
            return View(order);
        }
        [HttpGet]
        [Route("/mysales")]
        public IActionResult Sales()
        {
            string username = User.Identity.Name;
            var order = _db.Orders.Include(p => p.Post.UsernameNavigation).Include(p => p.BuyerNavigation).Where(p => p.Post.Username == username).OrderByDescending(p => p.OrderTime).ToList();
            return View(order);
        }
        [HttpGet]
        [Route("/orderdetail")]
        public IActionResult Details(int id)
        {
            string username = User.Identity.Name;
            var order = _db.Orders.Include(p => p.Post).Where(p => p.OrderId == id).FirstOrDefault();
            ViewBag.Order = order;
            var orderdetail = _db.OrderDetails.Where(p => p.OrderId == id);
            return View(orderdetail);
        }
    }
}
