using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using TMDT_MOHINHMACCA.Models;

namespace TMDT_MOHINHMACCA.ViewComponents
{
    public class CategoriesShopViewComponent : ViewComponent
    {
        public readonly ShopmaccaContext _db;
        public CategoriesShopViewComponent(ShopmaccaContext db)
        {
            _db = db;
        }
        public IViewComponentResult Invoke()
        {
            var categories = _db.Categories.Include(p=>p.Posts).OrderBy(p=>p.CateId).ToList();

            return View(categories);
        }
    }
}
