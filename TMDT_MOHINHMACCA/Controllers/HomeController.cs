using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMDT_MOHINHMACCA.Models;
using TMDT_MOHINHMACCA.ViewModels;
using X.PagedList;

namespace TMDT_MOHINHMACCA.Controllers
{
    public class HomeController : Controller
    {
        private readonly ShopmaccaContext _db;
        private readonly IMapper _mapper;

        public HomeController(ShopmaccaContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [Route("/")]
        [Route("/posts")]

        public IActionResult Index(int? page)
        {

            var categories = _db.Categories.Include(p => p.Posts).OrderBy(p => p.CateId);
            int pagesize = 9;
            int pagenumber = (page == null || page < 0) ? 1 : page.Value;
            var posts = _db.Posts
                .Include(p => p.Cate)
                .Include(p => p.UsernameNavigation)
                .Include(p => p.Orders)
                .Where(p => p.Status == "1").OrderByDescending(p=>p.PostApprovedtime)
                .ToList();

            var postsVMList = new List<PostVM>(); // Danh sách để lưu trữ các bài đăng đã được ánh xạ

            foreach (var post in posts)
            {
                var postVM = _mapper.Map<PostVM>(post); // Ánh xạ bài đăng sang đối tượng PostVM

                double totalStars = 0.0;
                int numberOfReviews = 0;

                foreach (var order in post.Orders)
                {
                    if (order.Star != null)
                    {
                        totalStars += order.Star.Value;
                        numberOfReviews++;
                    }
                }

                if (numberOfReviews > 0)
                {
                    double averageStar = totalStars / numberOfReviews;
                    postVM.Star = averageStar;
                    postVM.numberReview = numberOfReviews;
                }
                else
                {
                    postVM.Star = 0.0;
                    postVM.numberReview = 0;
                }
                int numberOfOrdersWithStatus3 = post.Orders.Count(o => o.Status == "3");
                postVM.numberOrder = numberOfOrdersWithStatus3;
                postVM.numberCmt = post.Orders.Count(p => p.Review != null);
                postsVMList.Add(postVM);
            }
            ViewBag.ListFiter = _db.Categories.ToList();
            PagedList<PostVM> list = new PagedList<PostVM>(postsVMList, pagenumber, pagesize);
            var tupleModel = new Tuple<IEnumerable<Category>, PagedList<PostVM>>(categories, list);
            return View(tupleModel);
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
        [Route("/404")]
        public IActionResult PageNotFound()
        {
            return View();
        }
        [Route("/usermanual")]
        public IActionResult Privacy()
        { 
            return View(); 
        }
    }
}
