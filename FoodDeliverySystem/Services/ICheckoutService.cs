using FoodDeliverySystem.ViewModels;

namespace FoodDeliverySystem.Services
{
    public interface ICheckoutService
    {
        Task<CheckoutResult> ProcessCheckoutAsync(
            string userId,
            CheckoutViewModel model,
            string userEmail,
            string callbackUrl);

        Task<PaymentCallbackResult> ProcessPaymentCallbackAsync(
            string reference,
            int orderId,
            int paymentId,
            string userId);
    }

    public class CheckoutResult
    {
        public bool Success { get; set; }

        public bool RedirectToPayment { get; set; }

        public string? PaymentUrl { get; set; }

        public string? ErrorMessage { get; set; }

        public int? OrderId { get; set; }

        public int? PaymentId { get; set; }
    }

    public class PaymentCallbackResult
    {
        public bool Success { get; set; }

        public string? ErrorMessage { get; set; }
    }
}