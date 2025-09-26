
using StilSepetiApp.DTO;
using StilSepetiApp.Enums;
namespace StilSepetiApp.Services
{
    public interface IPaymentService
    {
        Task<ServiceResult<PaymentResponsedto>> ProcessPaymentAsync(CreatePaymentRequestdto paymentRequest, int userId);
        Task<ServiceResult> VerifyPaymentAsync(string transactionId);
        Task<ServiceResult> RefundPaymentAsync(int paymentId, decimal amount);
        Task<PaymentResponsedto> GetPaymentStatusAsync(int paymentId);
    }
}
