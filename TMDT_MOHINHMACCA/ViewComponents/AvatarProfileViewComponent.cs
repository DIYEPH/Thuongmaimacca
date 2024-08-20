using Microsoft.AspNetCore.Mvc;
using TMDT_MOHINHMACCA.Models;
using TMDT_MOHINHMACCA.ViewModels;

namespace TMDT_MOHINHMACCA.ViewComponents
{
    public class AvatarProfileViewComponent : ViewComponent
    {
        private readonly ShopmaccaContext _db;
        public AvatarProfileViewComponent(ShopmaccaContext db)
        {
            _db = db;
        }
        public IViewComponentResult Invoke()
        {
            string username = User.Identity.Name;
            var account = _db.Accounts.Where(p => p.Username == User.Identity.Name).FirstOrDefault();
            ProfileVM profile = new ProfileVM();
            profile.Fullname = account.Fullname;
            profile.Avatar = account.Avatar;
            return View(profile);
        }
    }
}
