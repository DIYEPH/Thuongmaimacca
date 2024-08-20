using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMDT_MOHINHMACCA.Models;

namespace TMDT_MOHINHMACCA.Areas.Admin.ViewComponents
{
    public class PostApproveViewComponent : ViewComponent
    {
        private readonly ShopmaccaContext _db;
        public PostApproveViewComponent(ShopmaccaContext db)
        {
            _db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync(int? page, string? type, string searchInput)
        {
            if (type == null)
                type = "0";
            var post = await _db.Posts.Include(p => p.UsernameNavigation).Include(p => p.UsernameNavigation.Role).Include(p => p.Choose).Where(p => p.Status == "0").ToListAsync();
            return View(post);
        }
    }
}
