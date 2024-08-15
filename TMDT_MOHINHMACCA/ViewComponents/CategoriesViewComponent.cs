using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using TMDT_MOHINHMACCA.Models;

namespace TMDT_MOHINHMACCA.ViewComponents
{
    public class CategoriesViewComponent : ViewComponent
    {
        public readonly ShopmaccaContext _db;
        public CategoriesViewComponent(ShopmaccaContext db)
        {
            _db = db;
        }
        public IViewComponentResult Invoke()
        {
            var categories = _db.Categories.AsNoTracking().OrderBy(p=>p.CateId);
            return View(categories);
        }
    }
}
