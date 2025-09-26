using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StilSepetiApp.Data;
using StilSepetiApp.Models;
using System.Security.Claims;
using StilSepetiApp.Enums;
using StilSepetiApp.DTO;
namespace StilSepetiApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<OrderController> _logger;

        public OrderController(AppDbContext context, ILogger<OrderController> logger)
        {
            _context = context;
            _logger = logger;
        }
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequestdto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var userId = int.Parse(userIdClaim);

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
                return BadRequest("Sepetiniz boş.");

            // Stok kontrolü
            foreach (var item in cartItems)
            {
                if (item.Quantity > item.Product.Stock)
                    return BadRequest($"'{item.Product.Name}' ürünü için yeterli stok yok.");
            }

            var card = await _context.Cards
                .FirstOrDefaultAsync(c => c.UserId == userId && c.CardNumber == dto.CardNumber);

            if (card == null)
                return BadRequest("Kart bulunamadı.");

            if (card.CardPassword != dto.CardPassword)
                return BadRequest("Kart şifresi hatalı.");

            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.UserId == userId && a.Id == dto.AddressId);

            if (address == null)
                return BadRequest("Adres bulunamadı.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = new Order
                {
                    UserId = userId,
                    Status = OrderStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    StoredTotalAmount = cartItems.Sum(c => c.Product.Price * c.Quantity),
                    ShippingAddress = address.FullAddress,
                    Items = cartItems.Select(c => new OrderItem
                    {
                        ProductId = c.ProductId,
                        Quantity = c.Quantity,
                        Price = c.Product.Price
                    }).ToList(),
                };

                _context.Orders.Add(order);

                foreach (var item in cartItems)
                    item.Product.Stock -= item.Quantity;

                _context.CartItems.RemoveRange(cartItems);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Sipariş oluşturuldu: OrderId={OrderId}, UserId={UserId}", order.Id, userId);

                return Ok("Siparişiniz başarıyla oluşturuldu.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Checkout sırasında hata oluştu: UserId={UserId}", userId);
                return StatusCode(500, "Sipariş oluşturulurken bir hata oluştu.");
            }
        }


        [HttpPost("CreateFromCart")]
        public async Task<IActionResult> CreateOrderFromCart()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var userId = int.Parse(userIdClaim);

            var cartItems = await _context.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
                return BadRequest("Sepetiniz boş.");

            foreach (var cartItem in cartItems)
            {
                var product = await _context.Products.FindAsync(cartItem.ProductId);
                if (product == null)
                {
                    return BadRequest($"Ürün bulunamadı: ID {cartItem.ProductId}");
                }

                if (product.Stock < cartItem.Quantity)
                {
                    return BadRequest($"Ürün stoku yetersiz: {product.Name}. Mevcut stok: {product.Stock}, İstenen: {cartItem.Quantity}");
                }
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var cartItem in cartItems)
                {
                    var product = await _context.Products.FindAsync(cartItem.ProductId);
                    product!.Stock -= cartItem.Quantity;
                    _context.Products.Update(product);
                }

               
                var orderItems = cartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    Price = ci.Product!.Price
                }).ToList();

               
                var totalAmount = orderItems.Sum(oi => oi.Price * oi.Quantity);

                var order = new Order
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    Status = OrderStatus.Pending,
                    Items = orderItems,
                    StoredTotalAmount = totalAmount 
                };

                _context.Orders.Add(order);
                _context.CartItems.RemoveRange(cartItems);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Sipariş oluşturuldu: OrderId={OrderId}, UserId={UserId}, Total={Total}",
                    order.Id, userId, totalAmount);

                return Ok(new
                {
                    Message = "Sepetten sipariş oluşturuldu. Ödeme yapmak için /api/payment endpoint'ini kullanın.",
                    OrderId = order.Id,
                    TotalItems = order.Items.Count,
                    TotalAmount = order.TotalAmount, 
                    StoredTotalAmount = order.StoredTotalAmount, 
                    Status = order.Status.ToString(),
                    NextStep = "Ödeme yapmak için: POST /api/payment { orderId: " + order.Id + ", method: 'CreditCard' }"
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Sipariş oluşturulurken hata: UserId={UserId}", userId);
                return StatusCode(500, $"Sipariş oluşturulurken hata oluştu: {ex.Message}");
            }
        }

        [HttpPost("CreateManual")]
        public async Task<IActionResult> CreateOrderManual([FromBody] List<OrderItemCreatedto> items)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (items == null || !items.Any())
                return BadRequest("Sipariş için en az bir ürün girilmelidir.");

            foreach (var item in items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                {
                    return BadRequest($"Ürün bulunamadı: ID {item.ProductId}");
                }

                if (product.Stock < item.Quantity)
                {
                    return BadRequest($"Ürün stoku yetersiz: {product.Name}. Mevcut stok: {product.Stock}, İstenen: {item.Quantity}");
                }
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var userId = int.Parse(userIdClaim);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var item in items)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    product!.Stock -= item.Quantity;
                    _context.Products.Update(product);
                }

                var orderItems = new List<OrderItem>();
                decimal totalAmount = 0;

                foreach (var item in items)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    var orderItem = new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = product!.Price
                    };
                    orderItems.Add(orderItem);
                    totalAmount += product.Price * item.Quantity;
                }

                var order = new Order
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    Status = OrderStatus.Pending,
                    Items = orderItems,
                    StoredTotalAmount = totalAmount 
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Manuel sipariş oluşturuldu: OrderId={OrderId}, UserId={UserId}", order.Id, userId);

                return Ok(new
                {
                    Message = "Manuel sipariş oluşturuldu. Ödeme yapmak için /api/payment endpoint'ini kullanın.",
                    OrderId = order.Id,
                    TotalItems = order.Items.Count,
                    TotalAmount = order.TotalAmount, 
                    StoredTotalAmount = order.StoredTotalAmount, 
                    Status = order.Status.ToString(),
                    NextStep = "Ödeme yapmak için: POST /api/payment { orderId: " + order.Id + ", method: 'CreditCard' }"
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Manuel sipariş oluşturulurken hata: UserId={UserId}", userId);
                return StatusCode(500, $"Sipariş oluşturulurken hata oluştu: {ex.Message}");
            }
        }

        [HttpPost("Cancel/{orderId}")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var userId = int.Parse(userIdClaim);

            var order = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
                return NotFound("Sipariş bulunamadı.");

            var hasCompletedPayment = order.Payments.Any(p => p.Status == PaymentStatus.Completed);
            if (hasCompletedPayment)
            {
                return BadRequest("Ödeme yapılmış siparişler iptal edilemez. İade talebi oluşturabilirsiniz.");
            }

            var timeSinceOrder = DateTime.UtcNow - order.CreatedAt;
            if (timeSinceOrder.TotalHours > 24)
                return BadRequest("Sipariş 24 saati geçtiği için iptal edilemez.");

            if (order.Status != OrderStatus.Pending)
                return BadRequest("Yalnızca beklemede olan siparişler iptal edilebilir.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var item in order.Items)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        product.Stock += item.Quantity;
                        _context.Products.Update(product);
                    }
                }

                var pendingPayments = order.Payments.Where(p => p.Status == PaymentStatus.Pending);
                foreach (var payment in pendingPayments)
                {
                    payment.Status = PaymentStatus.Failed;
                    payment.ErrorMessage = "Sipariş iptal edildi";
                }

                order.Status = OrderStatus.Cancelled;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Sipariş iptal edildi: OrderId={OrderId}, UserId={UserId}", orderId, userId);

                return Ok("Sipariş başarıyla iptal edildi. Stoklar geri eklendi.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Sipariş iptal edilirken hata: OrderId={OrderId}, UserId={UserId}", orderId, userId);
                return StatusCode(500, $"Sipariş iptal edilirken hata oluştu: {ex.Message}");
            }
        }

        [HttpGet("{orderId}/payment-info")]
        public async Task<IActionResult> GetOrderPaymentInfo(int orderId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var userId = int.Parse(userIdClaim);

            var order = await _context.Orders
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
                return NotFound("Sipariş bulunamadı.");

            var paymentInfo = new
            {
                OrderId = order.Id,
                TotalAmount = order.TotalAmount, 
                StoredTotalAmount = order.StoredTotalAmount, 
                Status = order.Status.ToString(),
                Payments = order.Payments.Select(p => new
                {
                    p.Id,
                    p.Method,
                    p.Status,
                    p.Amount,
                    p.CreatedAt,
                    p.CompletedAt
                }),
                RequiresPayment = order.Status == OrderStatus.Pending &&
                                 !order.Payments.Any(p => p.Status == PaymentStatus.Completed)
            };

            return Ok(paymentInfo);
        }
        [HttpGet("myOrders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var orders = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            var result = orders.Select(o => new
            {
                o.Id,
                o.CreatedAt,
                o.Status,
                TotalAmount = o.StoredTotalAmount,
                ShippingAddress = o.ShippingAddress,
                Items = o.Items.Select(i => new
                {
                    i.Product.Name,
                    i.Product.ImageUrl,
                    i.Quantity,
                    i.Product.Price
                })
            });

            return Ok(result);
        }
    }
}

    
