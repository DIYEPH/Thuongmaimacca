using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMDT_MOHINHMACCA.Models;

namespace TMDT_MOHINHMACCA.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly ShopmaccaContext _db;
        private readonly IMapper _mapper;
        public UserController(ShopmaccaContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        [Route("/myprofile")]
        [HttpGet]
        public IActionResult Profile()
        {
            string? username = User.Identity.Name;
            Account account = new Account();
            if (username != null)
            {
                account = _db.Accounts.Include(a => a.Role).Include(a => a.Posts).SingleOrDefault(ac => ac.Username == username);
            }
            ViewBag.Balance = account.Money;
            return View(account);
        }
        [Route("/user")]
        [HttpGet]
        public IActionResult Profile(string username)
        {
            Account account = new Account();
            if (username != null)
            {
                account = _db.Accounts.Include(a => a.Role).Include(a => a.Posts).SingleOrDefault(ac => ac.Username == username);
            }
            return View(account);
        }
    }
}
