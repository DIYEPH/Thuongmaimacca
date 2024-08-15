using TMDT_MOHINHMACCA.Helpers;
using TMDT_MOHINHMACCA.ViewModels;

namespace TMDT_MOHINHMACCA.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _config;

        public VnPayService(IConfiguration config)
        {
            _config = config;
        }

        public string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model, string returnUrl)
        {
            var tick = DateTime.UtcNow.Ticks.ToString();
            var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";
            var fullReturnUrl = new Uri(new Uri(baseUrl), returnUrl).ToString();

            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", _config["VnPay:Version"]);
            vnpay.AddRequestData("vnp_Command", _config["VnPay:Command"]);
            vnpay.AddRequestData("vnp_TmnCode", _config["VnPay:TmnCode"]);
            vnpay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString()); // Số tiền thanh toán (nhân 100 lần)
            vnpay.AddRequestData("vnp_CreateDate", TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")).ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", _config["VnPay:CurrCode"]);
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", _config["VnPay:Locale"]);
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan cho don hang: " + model.Username + ":" + model.OrderId);
            vnpay.AddRequestData("vnp_OrderType", "other"); // Giá trị mặc định: other
            vnpay.AddRequestData("vnp_ReturnUrl", fullReturnUrl);
            vnpay.AddRequestData("vnp_TxnRef", tick); // Mã tham chiếu duy nhất
            var paymentUrl = vnpay.CreateRequestUrl(_config["VnPay:BaseUrl"], _config["VnPay:HashSecret"]);
            return paymentUrl;
        }

        public VnPaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var vnpay = new VnPayLibrary();
            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }

            var vnp_orderInfo = vnpay.GetResponseData("vnp_OrderInfo");
            var orderInfoParts = vnp_orderInfo.Split(':');
            int orderId = (int)Convert.ToInt64(orderInfoParts.LastOrDefault());
            string username = orderInfoParts[1].Trim().ToString();
            var vnp_TransactionId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
            var vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");
            long amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) /100; 

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _config["VnPay:HashSecret"]);
            if (!checkSignature)
            {
                return new VnPaymentResponseModel
                {
                    Success = false
                };
            }

            return new VnPaymentResponseModel
            {
                Amount =amount,
                Success = true,
                PaymentMethod = "VnPay",
                OrderDescription = vnp_OrderInfo,
                OrderId = orderId.ToString(),
                Username = username,
                TransactionId = vnp_TransactionId.ToString(),
                Token = vnp_SecureHash,
                VnPayResponseCode = vnp_ResponseCode
            };
        }
    }
}
