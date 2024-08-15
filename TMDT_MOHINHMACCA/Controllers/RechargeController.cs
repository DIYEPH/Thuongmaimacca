using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using TMDT_MOHINHMACCA.Helpers;
using TMDT_MOHINHMACCA.Models;
using TMDT_MOHINHMACCA.Services;
using TMDT_MOHINHMACCA.ViewModels;

namespace TMDT_MOHINHMACCA.Controllers
{
    [Authorize]
    public class RechargeController : Controller
    {
        private readonly ShopmaccaContext _db;
        private readonly IVnPayService _vnPayService;
        private readonly PaymentUrls _paymentUrls;
        public RechargeController(ShopmaccaContext db, IVnPayService vnPayService, IOptions<PaymentUrls> paymentUrls)
        {
            _db = db;
            _vnPayService = vnPayService;
            _paymentUrls = paymentUrls.Value;

        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [Route("/recharge")]
        public async Task<IActionResult> Index(RechargeVM model)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.Name == model.username)
                {
                    if (model.payment_method == "VNPay")
                    {
                        var vnPayModel = new VnPaymentRequestModel
                        {
                            Amount = model.deposit_amount,
                            CreatedDate = DateTime.Now,
                            Username = model.username,
                            Description = $"{model.username}_{model.payment_method}",
                            OrderId = model.deposit_amount
                        };
                        var redirectUrl = _vnPayService.CreatePaymentUrl(HttpContext, vnPayModel, _paymentUrls.Recharge);
                        return Redirect(redirectUrl);
                    }
                }
            }
            return BadRequest();
        }
        [HttpGet]
        [Route("/recharge/paymentcallback")]
        public IActionResult RechargeCallback()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["Message"] = $"Payment failed: {response.VnPayResponseCode}";
                return RedirectToAction("RechargeFail");
            }

            var account = _db.Accounts.FirstOrDefault(p => p.Username == response.Username);
            if (account == null)
            {
                return NotFound("Account not found");
            }
            
            var history = new Transactionhistory();
            history.Username = account.Username;
            history.Amountmoney = response.Amount;
            history.Initialbalance = account.Money;
            history.Finalbalance = account.Money + history.Amountmoney;
            DateTime utcNow = DateTime.UtcNow;
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, vietnamTimeZone);
            history.TransactionDate = vietnamTime;
            history.Content = "Nạp tiền vào tài khoản";
            history.TransactionType = "1";
            history.Payments = "1";
            _db.Transactionhistories.Add(history);
            account.Money += response.Amount;
            _db.SaveChanges();

            TempData["Message"] = "Recharge successful";
            return RedirectToAction("RechargeSuccess");
        }

        [HttpGet]
        [Route("/recharge/fail")]
        public IActionResult RechargeFail()
        {
            return View();
        }

        [HttpGet]
        [Route("/recharge/success")]
        public IActionResult RechargeSuccess()
        {
            return View();
        }
    }
}
