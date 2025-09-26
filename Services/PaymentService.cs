using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StilSepetiApp.Data;
using StilSepetiApp.DTO;
using StilSepetiApp.Enums;
using StilSepetiApp.Models;

namespace StilSepetiApp.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(AppDbContext context, ILogger<PaymentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult<PaymentResponsedto>> ProcessPaymentAsync(CreatePaymentRequestdto paymentRequest, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                
                var order = await _context.Orders
                    .Include(o => o.User)
                    .FirstOrDefaultAsync(o => o.Id == paymentRequest.OrderId && o.UserId == userId);

                if (order == null)
                    return ServiceResult<PaymentResponsedto>.FailureResult("Sipariş bulunamadı.");

                if (order.Status != OrderStatus.Pending)
                    return ServiceResult<PaymentResponsedto>.FailureResult("Sadece bekleyen siparişler için ödeme yapılabilir.");

               
                var payment = new Payment
                {
                    OrderId = paymentRequest.OrderId,
                    Amount = paymentRequest.Amount,
                    Method = paymentRequest.Method,
                    Status = PaymentStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                
                var paymentResult = await SimulatePaymentProcessing(paymentRequest, payment.Id);

                if (paymentResult.Success)
                {
                    payment.Status = PaymentStatus.Completed;
                    payment.TransactionId = paymentResult.TransactionId;
                    payment.CompletedAt = DateTime.UtcNow;

                    
                    order.Status = OrderStatus.Shipped;
                }
                else
                {
                    payment.Status = PaymentStatus.Failed;
                    payment.ErrorMessage = paymentResult.Message;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var response = new PaymentResponsedto
                {
                    Id = payment.Id,
                    OrderId = payment.OrderId,
                    Amount = payment.Amount,
                    Method = payment.Method,
                    Status = payment.Status,
                    CreatedAt = payment.CreatedAt,
                    TransactionId = payment.TransactionId
                };

                return ServiceResult<PaymentResponsedto>.SuccessResult(response, "Ödeme işlemi tamamlandı");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Ödeme işlemi sırasında hata oluştu: OrderId={OrderId}", paymentRequest.OrderId);
                return ServiceResult<PaymentResponsedto>.FailureResult("Ödeme işlemi sırasında bir hata oluştu.");
            }
        }

        private async Task<PaymentResult> SimulatePaymentProcessing(CreatePaymentRequestdto paymentRequest, int paymentId)
        {
          
            await Task.Delay(2000); 

           
            var random = new Random();
            if (random.Next(100) < 90)
            {
                return new PaymentResult
                {
                    Success = true,
                    TransactionId = $"TXN_{paymentId}_{DateTime.UtcNow:yyyyMMddHHmmss}"
                };
            }
            else
            {
                return new PaymentResult
                {
                    Success = false,
                    Message = "Ödeme işlemi banka tarafından reddedildi."
                };
            }
        }

        public async Task<ServiceResult> VerifyPaymentAsync(string transactionId)
        {
           
            return ServiceResult.SuccessBuilder("Ödeme doğrulandı");
        }

        public async Task<ServiceResult> RefundPaymentAsync(int paymentId, decimal amount)
        {
            
            return ServiceResult.SuccessBuilder("İade işlemi başlatıldı");
        }

        public async Task<PaymentResponsedto> GetPaymentStatusAsync(int paymentId)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.Id == paymentId);

            if (payment == null)
                return null;

            return new PaymentResponsedto
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                Amount = payment.Amount,
                Method = payment.Method,
                Status = payment.Status,
                CreatedAt = payment.CreatedAt,
                TransactionId = payment.TransactionId
            };
        }

        private class PaymentResult
        {
            public bool Success { get; set; }
            public string? TransactionId { get; set; }
            public string? Message { get; set; }
        }
    }
}