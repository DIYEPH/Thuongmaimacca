using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMDT_MOHINHMACCA.Models;
using TMDT_MOHINHMACCA.ViewModels;
using X.PagedList;

namespace TMDT_MOHINHMACCA.ViewComponents
{
    public class PostsShopViewComponent : ViewComponent
    {
        private readonly ShopmaccaContext _db;
        private readonly IMapper _mapper;
        public PostsShopViewComponent(ShopmaccaContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<IViewComponentResult> InvokeAsync(int? page, int? categoryid, string searchInput, int rangeInput, string? username)
        {
            int pagesize = 9;
            int pagenumber = (page == null || page < 0) ? 1 : page.Value;

            var postsQuery = _db.Posts.Include(p => p.Cate)
                         .Include(p => p.UsernameNavigation)
                         .Include(p => p.Orders)
                         .Where(p => p.Status == "1");

            if (categoryid != null)
            {
                postsQuery = postsQuery.Where(p => p.CateId == categoryid);
            }
            if (!string.IsNullOrEmpty(searchInput))
            {
                postsQuery = postsQuery.Where(p => p.Title.Contains(searchInput));
            }
            if (username != null)
            {
                postsQuery = postsQuery.Where(p => p.Username == username);
            }
            if (rangeInput != 0)
            {
                postsQuery = postsQuery.Where(p => p.PriceUp <= rangeInput && p.PriceTo >= rangeInput);
            }
            var posts = await postsQuery.ToListAsync();

            var postsVMList = new List<PostVM>();
            foreach (var post in posts)
            {
                var postVM = _mapper.Map<PostVM>(post);
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

            var list = new PagedList<PostVM>(postsVMList, pagenumber, pagesize);
            return View(list);
        }
    }
}
