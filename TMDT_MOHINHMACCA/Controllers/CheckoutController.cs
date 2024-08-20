using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TMDT_MOHINHMACCA.Helpers;
using TMDT_MOHINHMACCA.Models;
using TMDT_MOHINHMACCA.Services;
using TMDT_MOHINHMACCA.ViewModels;

namespace TMDT_MOHINHMACCA.Controllers
{
    [Authorize]

    public class CheckoutController : Controller
    {
        private readonly ShopmaccaContext _db;
        private readonly IVnPayService _vnPayservice;
        private readonly string _returnUrl;
        public CheckoutController(ShopmaccaContext db, IVnPayService vnPayservice, IOptions<PaymentUrls> paymentUrls)
        {
            _db = db;
            _vnPayservice = vnPayservice;
            _returnUrl = paymentUrls.Value.Checkout;
        }
        [HttpGet]
        [Route("/checkout")]
        public IActionResult Checkout(int? idchoose, int? postId)
        {
            if (idchoose != null)
            {
                try
                {
                    string? username = User.Identity?.Name;

                    if (string.IsNullOrEmpty(username))
                    {
                        return Unauthorized();
                    }
                    var account = _db.Accounts.FirstOrDefault(p => p.Username == username);
                    var choose = _db.Chooses.FirstOrDefault(p => p.ChooseId == idchoose);
                    if (choose == null)
                    {
                        return NotFound("Gói đã chọn không tồn tại");
                    }
                    bool accountHasEnoughMoney = false;
                    var chooseprice = choose.Price - choose.Price * (choose.Discount / 100.0);
                    if (account.Money >= (decimal)chooseprice)
                    {
                        accountHasEnoughMoney = true;
                    }
                    ViewBag.ChooseId = idchoose;
                    ViewBag.AccountHasEnoughMoney = accountHasEnoughMoney;
                    ViewBag.MyBalance = account.Money;
                    ViewBag.AmountToPay = chooseprice;
                    return View();
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Lỗi kiểm tra tài khoản: {ex.Message}");
                }
            }
            return View();
        }

        [HttpPost]
        [Route("/checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutVM model)
        {
            if (ModelState.IsValid)
            {
                var choose = _db.Chooses.Where(p => p.ChooseId == model.ChooseId).FirstOrDefault();
                var account = _db.Accounts.Where(p => p.Username == model.Buyer).FirstOrDefault();
                if (model.payment == "VNPay")
                {
                    var vnPayModel = new VnPaymentRequestModel
                    {
                        Amount = (double)(choose.Price - choose.Price * (choose.Discount / 100.0)),
                        CreatedDate = DateTime.Now,
                        Description = $"{model.Buyer}_{model.payment}",
                        Username = model.Buyer,
                        OrderId = model.PostId
                    };
                    var redirectUrl = _vnPayservice.CreatePaymentUrl(HttpContext, vnPayModel, _returnUrl);
                    return Ok(redirectUrl);
                }
                else if (model.payment == "Balance")
                {
                    if (model.Buyer == User.Identity.Name)
                    {
                        var chooseprice = choose.Price - choose.Price * (choose.Discount / 100.0);
                        var post = _db.Posts.Find(model.PostId);
                        if (account.Money >= (decimal)chooseprice)
                        {
                            var history = new Transactionhistory();
                            history.Username = account.Username;
                            history.Amountmoney = (decimal)chooseprice;
                            history.Initialbalance = account.Money;
                            history.Finalbalance = account.Money - history.Amountmoney;
                            DateTime utcNow = DateTime.UtcNow;
                            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, vietnamTimeZone);
                            history.TransactionDate = vietnamTime;
                            history.TransactionType = "2";
                            history.Content = "Thanh toán cho mã bài đăng: " + post.PostId;
                            history.Payments = "0";
                            _db.Transactionhistories.Add(history);
                            if (post.Status == "c")
                            {
                                account.Money -= (decimal)chooseprice;
                                post.Status = "0";
                            }

                            _db.SaveChanges();
                            return Ok();
                        }
                    }
                    return BadRequest();
                }
            }
            return BadRequest();
        }
        [HttpGet]
        [Route("/checkout/paymentfail")]
        public IActionResult PaymentFail()
        {
            return View();
        }

        [HttpGet]
        [Route("/checkout/success")]
        public IActionResult PaymentSuccess()
        {
            return View();
        }
        [Route("/checkout/paymentcallback")]
        public IActionResult PaymentCallBack()
        {
            var response = _vnPayservice.PaymentExecute(Request.Query);
            var post = _db.Posts.Find(Convert.ToInt32(response.OrderId));
            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["Message"] = $"Lỗi thanh toán VN Pay: {response.VnPayResponseCode}";
                if (post != null)
                    _db.Posts.Remove(post);
                _db.SaveChanges();
                return RedirectToAction("PaymentFail");
            }

            if (post.Status == "c")
            {
                post.Status = "0";
            }
            var account = _db.Accounts.Find(User.Identity.Name);
            var history = new Transactionhistory();
            history.Username = account.Username;
            history.Amountmoney = response.Amount;
            history.Initialbalance = account.Money;
            history.TransactionType = "2";
            history.Finalbalance = account.Money;
            DateTime utcNow = DateTime.UtcNow;
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, vietnamTimeZone);
            history.TransactionDate = vietnamTime;
            history.Content = "Thanh toán cho mã bài đăng: " + post.PostId;
            history.Payments = "1";
            _db.Transactionhistories.Add(history);
            _db.SaveChanges();
            TempData["Message"] = $"Thanh toán VNPay thành công";
            return RedirectToAction("PaymentSuccess");
        }
    }
}
