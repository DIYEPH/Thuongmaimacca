using TMDT_MOHINHMACCA.ViewModels;

namespace TMDT_MOHINHMACCA.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model, string returnUrl);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
