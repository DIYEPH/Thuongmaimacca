using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;
using TMDT_MOHINHMACCA.Helpers;
using TMDT_MOHINHMACCA.Models;
using TMDT_MOHINHMACCA.ViewModels;

namespace TMDT_MOHINHMACCA.Controllers
{
    public class AccessController : Controller
    {
        private readonly ShopmaccaContext _db;
        private readonly IMapper _mapper;
        public AccessController(ShopmaccaContext context, IMapper mapper)
        {
            _db = context;
            _mapper = mapper;
        }
        [Route("/login")]
        [HttpGet]
        public IActionResult Login(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        public async Task LoginWithGoogle()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("GoogleResponse")
                });
        }
        public async Task<IActionResult> GoogleResponse()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
                return BadRequest(); // Xử lý lỗi đăng nhập không thành công

            var claims = authenticateResult.Principal?.Identities?.FirstOrDefault()?.Claims;

            if (claims == null || !claims.Any())
                return BadRequest("Google claims not found"); // Xử lý lỗi khi không tìm thấy thông tin từ Google

            var googleId = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            var email = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            var fullname = claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;

            if (googleId == null || email == null)
                return BadRequest("Google ID or email claim not found"); // Xử lý lỗi khi không tìm thấy Google ID hoặc email

            var account = _db.Accounts.SingleOrDefault(ac => ac.GoogleId == googleId || ac.Email == email);
            if (account == null)
            {
                // Kiểm tra xem người dùng có tồn tại với email này không
                var existingAccount = _db.Accounts.SingleOrDefault(ac => ac.Email == email);
                if (existingAccount != null)
                {
                    // Nếu người dùng đã đăng ký thông thường trước đó, cập nhật GoogleId của tài khoản này
                    existingAccount.GoogleId = googleId;
                    await _db.SaveChangesAsync();

                    // Sử dụng tài khoản đã tồn tại để đăng nhập
                    account = existingAccount;
                }
                else
                {
                    // Tạo tài khoản mới nếu không tìm thấy
                    account = new Account
                    {
                        Username = email,
                        Email = email,
                        Fullname = fullname ?? email,
                        RoleId = 2, // Gán role mặc định (người dùng bình thường)
                        Validity = true,
                        Password = "", // Không cần mật khẩu vì dùng đăng nhập Google
                        Randomkey = "",
                        Money = 0,
                        Avatar = "https://bootdey.com/img/Content/avatar/avatar7.png",
                        Gender = "0",
                        Signupdate = DateTime.Now,
                        GoogleId = googleId
                    };
                    
                    var history = new Transactionhistory();
                    history.Username = account.Username;
                    history.Amountmoney = 0;
                    history.Initialbalance=account.Money;
                    history.Finalbalance=account.Money+=history.Amountmoney;
                    history.TransactionDate = DateTime.Now;
                    history.TransactionType = "0";
                    history.Content = "Thưởng đăng ký";
                    history.Payments = "0";
                    _db.Accounts.Add(account);
                    _db.Transactionhistories.Add(history);
                    await _db.SaveChangesAsync();
                    await Task.Run(() => {
                        var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";
                        var salesUrl = $"{baseUrl}/";
                        var receive = _db.Accounts.Find(account.Email);
                        XMail.Send(email, "Xác nhận đăng ký", $"Chào bạn,\n\nChúc mừng! Bạn đã đăng ký tài khoản trên trang web của chúng tôi thành công.\n\nChúng tôi rất vui mừng khi chào đón bạn vào cộng đồng của chúng tôi. Tài khoản của bạn đã được kích hoạt và bạn có thể bắt đầu sử dụng các tính năng của trang web ngay lập tức.\n\nNếu bạn có bất kỳ câu hỏi hoặc cần hỗ trợ, vui lòng trả lời email này hoặc liên hệ với chúng tôi qua trang web.\n\nTrân trọng,\nNhóm hỗ trợ của chúng tôi\n . <a href='{salesUrl}'>Nhấn vào đây</a> để xem chi tiết.");
                        TempData["SuccessMessage"] = "Đăng ký tài khoản thành công!";
                    });
                }
            }

            // Tạo Claims cho người dùng
            var appClaims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, account.Username),
    new Claim(ClaimTypes.Email, account.Email),
    new Claim(ClaimTypes.Name, account.Username),
    new Claim("Fullname", account.Fullname),
    new Claim(ClaimTypes.Role, account.RoleId.ToString())
};

            var claimsIdentity = new ClaimsIdentity(appClaims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30),
                IsPersistent = true
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);
            if (account.RoleId == 1)
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }         
               return Redirect("/");
        }
        [Route("/login")]
        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginVM model, string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            var account = _db.Accounts.Include(a => a.Role)
             .SingleOrDefault(ac => ac.Username == model.Username);
            if (account == null)
                ViewData["ErrorMessage"] = "Không có tài khoản này!";
            else
            {
                if (account.Validity == false)
                    ViewData["ErrorMessage"] = "Tài khoản đã bị khóa. Vui lòng liên hệ Admin!";
                else
                {
                    if (account.Password != model.Password.ToMd5Hash(account.Randomkey))
                        ViewData["ErrorMessage"] = "Mật khẩu không chính xác!";
                    else
                    {
                        var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.NameIdentifier, account.Username),
                                new Claim(ClaimTypes.Email, account.Email),
                                new Claim(ClaimTypes.Name, account.Username),
                                new Claim("Fullname", account.Fullname),
                                new Claim(ClaimTypes.Role, account.RoleId.ToString())
                            };
                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = model.RememberMe,
                            ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : (DateTimeOffset?)null
                        };
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);

                        if (account.RoleId == 1)
                        {
                            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                        }

                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return Redirect("/");
                        }
                    }
                }
            }
            return View();
        }


        [Route("/signup")]
        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }
        [Route("/signup")]
        [HttpPost]
        public IActionResult SignupAsync(SignupVM model)
        {
            if (ModelState.IsValid)
            {
                var existingAccount = _db.Accounts.FirstOrDefault(m => m.Username == model.Username);
                if (existingAccount != null)
                {
                    @ViewData["ErrorMessage"] = "Tên đăng nhập đã tồn tại";
                    return View(model);
                }
                var existingEmail = _db.Accounts.FirstOrDefault(m => m.Email == model.Email);
                if (existingEmail != null)
                {
                    @ViewData["ErrorMessage"] = "Email đã tồn tại";
                    return View(model);
                }

                var account = _mapper.Map<Account>(model);
                account.Randomkey = MyUtil.GenerateRandomKey();
                account.Password = model.Password.ToMd5Hash(account.Randomkey);
                account.Validity = true;
                account.RoleId = 2;
                account.Gender = "0";
                account.Avatar = "https://bootdey.com/img/Content/avatar/avatar7.png";
                account.Money = 0;
                account.Signupdate = DateTime.Now;
                var history = new Transactionhistory();
                history.Username = account.Username;
                history.Amountmoney = 0;
                history.TransactionType = "0";
                history.Initialbalance = account.Money;
                history.Finalbalance = account.Money += history.Amountmoney;
                history.TransactionDate = DateTime.Now;
                history.Content = "Thưởng đăng ký";
                history.Payments = "0";
                _db.Accounts.Add(account);
                _db.Transactionhistories.Add(history);
                _db.SaveChanges();
                string email = model.Email;
                XMail.Send(email, "Xác nhận đăng ký", "Chào bạn,\n\nChúc mừng! Bạn đã đăng ký tài khoản trên trang web của chúng tôi thành công.\n\nChúng tôi rất vui mừng khi chào đón bạn vào cộng đồng của chúng tôi. Tài khoản của bạn đã được kích hoạt và bạn có thể bắt đầu sử dụng các tính năng của trang web ngay lập tức.\n\nNếu bạn có bất kỳ câu hỏi hoặc cần hỗ trợ, vui lòng trả lời email này hoặc liên hệ với chúng tôi qua trang web.\n\nTrân trọng,\nNhóm hỗ trợ của chúng tôi");
                TempData["SuccessMessage"] = "Đăng ký tài khoản thành công!";

                return RedirectToAction("Login", "Access");
            }
            return View();
        }
        [Route("/logout")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            return Redirect("/");
        }

        [AllowAnonymous]
        [Route("/forgot")]
        public ActionResult Forgot()
        {
            return View();
        }
        [Route("/forgot")]
        [AllowAnonymous, HttpPost]
        public ActionResult Forgot(string UserName, string Email)
        {
            try
            {
                var cust = _db.Accounts
                    .Single(c => c.Username == UserName && c.Email == Email);

                var user = _db.Accounts.Find(UserName);
                string TokenCode = GenerateRandomToken(8);
                user.Password = TokenCode.ToMd5Hash(user.Randomkey);
                _db.SaveChanges();
                XMail.Send(Email, "Token Code", TokenCode);
                TempData["UserName"] = UserName;
                return View("Reset");
            }
            catch
            {
                ViewBag.UserName = UserName;
                ViewBag.Email = Email;
                ViewData["ErrorMessage"] = "Sai thông tin tài khoản !";
                return View();
            }
        }
        [AllowAnonymous, HttpGet]
        [Route("/reset")]
        public ActionResult Reset()
        {
            ViewBag.UserName = TempData["UserName"];
            return View();
        }
        [AllowAnonymous, HttpPost]
        [Route("/reset")]
        public ActionResult Reset(string UserName, string TokenCode, string NewPassword)
        {
            try
            {
                var user = _db.Accounts.Find(UserName);
                if (user != null)
                {
                    if (user.Password == TokenCode.ToMd5Hash(user.Randomkey))
                    {
                        user.Password = NewPassword.ToMd5Hash(user.Randomkey);
                        _db.SaveChanges();
                        TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Mã khôi phục không đúng!";
                        return RedirectToAction("Reset");
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Tên đăng nhập không hợp lệ!";
                    return RedirectToAction("Reset");
                }
            }
            catch
            {
                TempData["SuccessMessage"] = "Đổi mật khẩu không thành công!";
                return RedirectToAction("Login");
            }

        }

        public static string GenerateRandomToken(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
