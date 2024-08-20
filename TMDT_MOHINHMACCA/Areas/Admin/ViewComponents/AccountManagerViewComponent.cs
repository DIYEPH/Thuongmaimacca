using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMDT_MOHINHMACCA.Models;

namespace TMDT_MOHINHMACCA.Areas.Admin.ViewComponents
{
    public class AccountManagerViewComponent : ViewComponent
    {
        private readonly ShopmaccaContext _db;
        public AccountManagerViewComponent(ShopmaccaContext db)
        {
            _db = db;
        }
        public async Task<IViewComponentResult> InvokeAsync(int? page, string? type, string searchInput)
        {
            var accounts = await _db.Accounts.Include(p => p.Role).ToListAsync();
            return View(accounts);
        }
    }
}
