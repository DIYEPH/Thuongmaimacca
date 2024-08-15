using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TMDT_MOHINHMACCA.Models;

namespace TMDT_MOHINHMACCA.Controllers
{
    [Authorize]
    public class HistoryTransactionController : Controller
    {
        private readonly ShopmaccaContext _db;
        public HistoryTransactionController(ShopmaccaContext db)
        {
            _db = db;
        }
        [Route("/myhistorytransaction")]
        public IActionResult Index()
        {
            var user = _db.Accounts.Find(User.Identity.Name);
            var histories= _db.Transactionhistories.Where(p=>p.Username==user.Username).OrderBy(p=>p.TransactionDate).ToList();
            return View(histories);
        }
    }
}
