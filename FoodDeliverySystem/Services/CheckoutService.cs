using FoodDeliverySystem.Data;
using FoodDeliverySystem.Models;
using FoodDeliverySystem.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliverySystem.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly ApplicationDbContext _context;
        private readonly PaystackService _paystackService;

        public CheckoutService(
            ApplicationDbContext context,
            PaystackService paystackService)
        {
            _context = context;
            _paystackService = paystackService;
        }

        // =========================================================
        // PROCESS CHECKOUT
        // =========================================================

        public async Task<CheckoutResult> ProcessCheckoutAsync(
     string userId,
     CheckoutViewModel model,
     string userEmail,
     string callbackUrl)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.FoodItem)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                return new CheckoutResult
                {
                    Success = false,
                    ErrorMessage = "Your cart is empty."
                };
            }

            using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                decimal subtotal = 0;
                decimal deliveryFee = 1000;

                // Calculate subtotal
                foreach (var item in cart.CartItems)
                {
                    if (item.FoodItem == null)
                    {
                        return new CheckoutResult
                        {
                            Success = false,
                            ErrorMessage =
                                "One or more items in your cart are no longer available."
                        };
                    }

                    if (!item.FoodItem.IsAvailable)
                    {
                        return new CheckoutResult
                        {
                            Success = false,
                            ErrorMessage =
                                $"{item.FoodItem.Name} is currently unavailable."
                        };
                    }

                    subtotal +=
                        item.Quantity * item.FoodItem.Price;
                }

                decimal grandTotal = subtotal + deliveryFee;

                // =================================================
                // CREATE ORDER
                // =================================================

                var order = new Order
                {
                    CustomerId = userId,
                    DeliveryAddress = model.DeliveryAddress,
                    PhoneNumber = model.PhoneNumber,
                    OrderNote = model.OrderNote,

                    PaymentMethod = model.PaymentMethod,

                    SubTotal = subtotal,
                    DeliveryFee = deliveryFee,
                    TotalAmount = grandTotal,

                    Status = model.PaymentMethod == "Cash on Delivery"
                        ? "Pending"
                        : "Pending Payment",

                    IsPaid = false
                };

                _context.Orders.Add(order);

                await _context.SaveChangesAsync();

                // =================================================
                // CREATE ORDER ITEMS
                // =================================================

                foreach (var item in cart.CartItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        FoodItemId = item.FoodItemId,
                        Quantity = item.Quantity,
                        Price = item.FoodItem!.Price,
                        TotalPrice =
                            item.Quantity * item.FoodItem.Price
                    };

                    _context.OrderItems.Add(orderItem);
                }

                // =================================================
                // PAYMENT STATUS
                // =================================================

                string paymentStatus;

                if (model.PaymentMethod == "Cash on Delivery")
                {
                    paymentStatus = "Pending Collection";
                }
                else
                {
                    paymentStatus = "Awaiting Payment";
                }

                // =================================================
                // CREATE PAYMENT
                // =================================================

                var payment = new Payment
                {
                    OrderId = order.Id,
                    Amount = grandTotal,
                    PaymentMethod = model.PaymentMethod,
                    PaymentStatus = paymentStatus,

                    ExpiresAt =
                        DateTime.Now.AddMinutes(15),

                    IsExpired = false
                };

                _context.Payments.Add(payment);

                await _context.SaveChangesAsync();

                // =================================================
                // CREATE ADMIN NOTIFICATION
                // =================================================

                _context.Notifications.Add(new Notification
                {
                    Title = "New Order",
                    Message =
                        $"Order #{order.Id} has been placed.",
                    Type = "Order",
                    Icon = "bi bi-bag-check-fill",
                    Url =
                        $"/Order/AdminDetails/{order.Id}",
                    IsRead = false,
                    CreatedAt = DateTime.Now
                });

                await _context.SaveChangesAsync();

                // =================================================
                // ONLINE PAYMENT
                // =================================================

                if (model.PaymentMethod == "Pay Online")
                {
                    var paymentUrl =
                       await _paystackService.InitializePayment(
                      userEmail,
                      grandTotal,
                      callbackUrl);

                    if (!string.IsNullOrEmpty(paymentUrl))
                    {
                        await transaction.CommitAsync();

                        return new CheckoutResult
                        {
                            Success = true,
                            RedirectToPayment = true,
                            PaymentUrl = paymentUrl,
                            OrderId = order.Id,
                            PaymentId = payment.Id
                        };
                    }

                    await transaction.RollbackAsync();

                    return new CheckoutResult
                    {
                        Success = false,
                        ErrorMessage =
                            "Unable to connect to Paystack."
                    };
                }

                // =================================================
                // CASH ON DELIVERY
                // =================================================

                if (model.PaymentMethod == "Cash on Delivery")
                {
                    _context.CartItems.RemoveRange(
                        cart.CartItems);

                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return new CheckoutResult
                    {
                        Success = true,
                        RedirectToPayment = false,
                        OrderId = order.Id,
                        PaymentId = payment.Id
                    };
                }

                await transaction.CommitAsync();

                return new CheckoutResult
                {
                    Success = true,
                    RedirectToPayment = false,
                    OrderId = order.Id,
                    PaymentId = payment.Id
                };
            }
            catch
            {
                await transaction.RollbackAsync();

                return new CheckoutResult
                {
                    Success = false,
                    ErrorMessage =
                        "Unable to complete your order. Please try again."
                };
            }
        }


        // =========================================================
        // PROCESS PAYMENT CALLBACK
        // =========================================================

        public async Task<PaymentCallbackResult>
            ProcessPaymentCallbackAsync(
                string reference,
                int orderId,
                int paymentId,
                string userId)
        {
            if (string.IsNullOrEmpty(reference))
            {
                return new PaymentCallbackResult
                {
                    Success = false,
                    ErrorMessage =
                        "Payment reference was not received."
                };
            }

            var verifyResult =
                await _paystackService.VerifyPayment(reference);

            if (verifyResult == null ||
                verifyResult.status != "success")
            {
                return new PaymentCallbackResult
                {
                    Success = false,
                    ErrorMessage =
                        "Payment verification failed."
                };
            }

            var order =
                await _context.Orders.FindAsync(orderId);

            if (order == null)
            {
                return new PaymentCallbackResult
                {
                    Success = false,
                    ErrorMessage =
                        "Order not found."
                };
            }

            var payment =
                await _context.Payments.FindAsync(paymentId);

            if (payment == null)
            {
                return new PaymentCallbackResult
                {
                    Success = false,
                    ErrorMessage =
                        "Payment record not found."
                };
            }

            order.IsPaid = true;
            order.Status = "Pending";

            payment.PaymentStatus = "Paid";

            // Clear customer's cart only after
            // successful payment verification.
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c =>
                    c.UserId == userId);

            if (cart != null)
            {
                _context.CartItems.RemoveRange(
                    cart.CartItems);
            }

            await _context.SaveChangesAsync();

            return new PaymentCallbackResult
            {
                Success = true
            };
        }
    }
}