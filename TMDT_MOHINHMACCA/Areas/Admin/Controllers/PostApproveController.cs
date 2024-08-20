using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMDT_MOHINHMACCA.Helpers;
using TMDT_MOHINHMACCA.Models;

namespace TMDT_MOHINHMACCA.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/PostApprove")]
    [Authorize(Roles = "1")]
    public class PostApproveController : Controller
    {
        private readonly ShopmaccaContext _db;
        public PostApproveController(ShopmaccaContext db)
        {
            _db = db;
        }
        public Account account
        {
            get
            {
                return _db.Accounts.Find(User.Identity.Name);
            }
        }
        [Route("index")]
        public IActionResult Index()
        {
            var posts = _db.Posts.Where(p => p.Status == "0").ToList();
            return View(posts);
        }

        [Route("Accept")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(int id)
        {
            var post = _db.Posts.Find(id);
            if (post.Status == "0")
                post.Status = "1";
            post.PostApprovedtime = DateTime.Now;

            _db.SaveChanges();
            Task.Run(() =>
            {
                var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";
                var salesUrl = $"{baseUrl}/myprofile";
                var receive = _db.Accounts.Find(post.Username);
                XMail.Send(receive.Email, "Bài đăng của bạn đã được duyệt!", $"Bài đăng {post.Title}' đã được duyệt. Người thực hiện: {account.Fullname} <a href='{salesUrl}'>Nhấn vào đây</a> để xem chi tiết.");
            });
            return RedirectToAction(nameof(Index));
        }
        [Route("Disaccept")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Disapprove(int id)
        {
            var admin = _db.Accounts.Find(User.Identity.Name);
            var post = _db.Posts.Include(p => p.Choose).Where(p => p.PostId == id).SingleOrDefault();
            if (post.Status == "0")
                post.Status = "k";
            var refunds = post.Choose.Price;
            var account = _db.Accounts.Where(p => p.Username == post.Username).FirstOrDefault();

            var history = new Transactionhistory();
            history.Username = account.Username;
            history.Amountmoney = 2000;
            history.Initialbalance = account.Money;
            history.Finalbalance = account.Money + history.Amountmoney;
            DateTime utcNow = DateTime.UtcNow;
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, vietnamTimeZone);
            history.TransactionDate = vietnamTime;
            history.Content = "Hoàn trả bài đăng, Mã bài đăng: " + id;
            history.TransactionType = "4";
            history.Payments = "0";
            _db.Transactionhistories.Add(history);
            account.Money += refunds;
            _db.SaveChanges();
            Task.Run(() =>
            {
                var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";
                var salesUrl = $"{baseUrl}/myprofile";
                var receive = _db.Accounts.Find(post.Username);
                XMail.Send(receive.Email, "Bài đăng của bạn không được duyệt!", $"Bài đăng '{post.Title}' đã không duyệt. Số tiền đã được hoàn trả vào tài khoản của bạn. Người thực hiện: {admin.Fullname} <a href='{salesUrl}'>Nhấn vào đây</a> để xem chi tiết.");
            });
            return RedirectToAction(nameof(Index));
        }
    }
}
