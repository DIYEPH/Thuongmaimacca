using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMDT_MOHINHMACCA.Models;

namespace TMDT_MOHINHMACCA.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Dashboard")]
    [Route("Admin/")]
    [Authorize(Roles = "1")]
    public class DashboardController : Controller
    {
        private readonly ShopmaccaContext _db;
        public DashboardController(ShopmaccaContext db)
        {
            _db = db;
        }
        [Route("Index")]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            DateTime StartDate = DateTime.Today.AddDays(-6);
            DateTime EndDate = DateTime.Today;

            List<Post> PostsList = await _db.Posts.Include(x => x.Cate).Where(p => p.Status == "1").ToListAsync();

            int TotalAccount = await _db.Accounts.CountAsync();
            ViewBag.TotalAccount = TotalAccount;

            int TotalPost = await _db.Posts.CountAsync();
            ViewBag.TotalPost = TotalPost;
            int TotalPostAp = await _db.Posts.Where(p => p.Status == "1").CountAsync();
            ViewBag.TotalPostAp = TotalPostAp;

            int TotalComplete = await _db.Orders.Where(p => p.Status == "3").CountAsync();
            ViewBag.TotalComplete = TotalComplete;
            IEnumerable<Transactionhistory> history = _db.Transactionhistories.ToList();
            decimal doanhthu = (decimal)history.Sum(p => p.Finalbalance - p.Initialbalance);
            ViewBag.Doanhthu = doanhthu;
            ViewBag.DoughnutChartData = PostsList.GroupBy(p => p.CateId).Select(k => new
            {
                category = k.First().Cate.CateName,
                count = k.Count()
            })
                .OrderByDescending(k => k.count);

            List<CountPost> CountPost = PostsList
            .GroupBy(g => g.PostApprovedtime)
            .Select(k => new CountPost()
            {
                day = k.First().PostApprovedtime?.ToString("dd/MM"),
                count = k.Count()
            })
            .ToList();

            string[] Last7Days = Enumerable.Range(0, 7)
    .Select(i => StartDate.AddDays(i).ToString("dd/MM"))
    .ToArray();

            ViewBag.CountPostForDay = from day in Last7Days
                                      join post in CountPost on day equals post.day into dayPostJoined
                                      from post in dayPostJoined.DefaultIfEmpty()
                                      select new
                                      {
                                          day = day,
                                          count = post == null ? 0 : post.count
                                      };
            var lineChartData = new List<int> { 65, 59, 80, 81, 56, 55, 40 };
            ViewBag.LineChartData = lineChartData;
            return View();
        }
    }
}

public class CountPost
{
    public string day { get; set; }
    public int count { get; set; }
}