using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMDT_MOHINHMACCA.Models;
using TMDT_MOHINHMACCA.ViewModels;

namespace TMDT_MOHINHMACCA.ViewComponents
{
    public class FeaturedShopViewComponent : ViewComponent
    {
        private readonly ShopmaccaContext _db;
        private readonly IMapper _mapper;
        public FeaturedShopViewComponent(ShopmaccaContext db, IMapper mapper)
        {
            _mapper = mapper;
            _db = db;
        }
        public IViewComponentResult Invoke()
        {
            var posts = _db.Posts
                .Include(p => p.Cate)
                .Include(p => p.UsernameNavigation)
                .Include(p => p.Orders)
                .Where(p => p.Status == "1")
                .ToList();

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
            var topRatedPosts = postsVMList.OrderByDescending(p => p.Star).Take(4).ToList();
            return View(topRatedPosts);
        }
    }
}
