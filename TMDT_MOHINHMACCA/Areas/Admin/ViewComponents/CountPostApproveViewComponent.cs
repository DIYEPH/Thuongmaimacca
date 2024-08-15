using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMDT_MOHINHMACCA.Models;

namespace TMDT_MOHINHMACCA.Areas.Admin.ViewComponents
{
    public class CountPostApproveViewComponent: ViewComponent
    {
        private readonly ShopmaccaContext _db;
        public CountPostApproveViewComponent(ShopmaccaContext db)
        {
            _db = db;
        }
        public IViewComponentResult Invoke()
        {         
            var post = _db.Posts.Include(p=>p.UsernameNavigation).ToList();
            return View(post);
        }
    }
}
