using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TMDT_MOHINHMACCA.Models;

namespace TMDT_MOHINHMACCA.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Account")]
    [Authorize(Roles = "1")]
    public class AccountController : Controller
    {
        private readonly ShopmaccaContext _db;
        public AccountController(ShopmaccaContext db)
        {
            _db = db;
        }
        [Route("Index")]
        public IActionResult Index(string error)
        {
            if (!string.IsNullOrEmpty(error))
            {
                ViewData["Error"] = error;
            }
            return View();
        }
        [Route("Band")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Band(string username)
        {
            var account = _db.Accounts.Find(username);
            if (username == User.Identity.Name)
                return RedirectToAction(nameof(Index), new { error = "Không thể khóa tài khoản của chính mình!" });
            if (account != null && account.Validity == true && username != User.Identity.Name)
                account.Validity = false;
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [Route("Unband")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Unband(string username)
        {
            var account = _db.Accounts.Find(username);
            if (account != null && account.Validity == false)
                account.Validity = true;
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [Route("AddAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddAdmin(string username)
        {
            var account = _db.Accounts.Find(username);
            if (account != null && account.RoleId == 2)
                account.RoleId = 1;
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [Route("Delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string username)
        {
            var account = _db.Accounts.Find(username);
            if (username == User.Identity.Name)
                return RedirectToAction(nameof(Index), new { error = "Không thể xóa tài khoản của chính mình!" });
            try
            {
                if (account != null && username != User.Identity.Name)
                    _db.Remove(account);
                _db.SaveChanges();
            }
            catch
            {
                return RedirectToAction(nameof(Index), new { error = "Tài khoản trên đã có dữ liệu, không thể xoas!" });
            }
            return RedirectToAction(nameof(Index));
        }
        [Route("DeleteAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAdmin(string username)
        {
            var account = _db.Accounts.Find(username);
            if (username == User.Identity.Name)
                return RedirectToAction(nameof(Index), new { error = "Không thể hủy admin tài khoản chính mình!" });
            if (account != null && account.RoleId == 1 && username != User.Identity.Name)
                account.RoleId = 2;
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
