using Microsoft.AspNetCore.Mvc;
using TMDT_MOHINHMACCA.Models;
using TMDT_MOHINHMACCA.ViewModels;

namespace TMDT_MOHINHMACCA.ViewComponents
{
    public class BalancesViewComponent : ViewComponent
    {
        private readonly ShopmaccaContext _db;
        public BalancesViewComponent(ShopmaccaContext db)
        {
            _db = db;
        }
        public IViewComponentResult Invoke()
        {
            string username = User.Identity.Name;
            var balanceAmount = _db.Accounts
    .Where(p => p.Username == username)
    .Select(p => p.Money)
    .FirstOrDefault();
            var formattedBalance = string.Format("{0:C}", balanceAmount);
            BalanceVM balance = new BalanceVM { money = (decimal)balanceAmount };
            return View(balance);
        }
    }
}
