using System.ComponentModel.DataAnnotations;

namespace FoodDeliverySystem.ViewModels
{
    public class CheckoutViewModel
    {
        [Required]
        [Display(Name = "Delivery Address")]
        public string DeliveryAddress { get; set; } = string.Empty;

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Display(Name = "Order Note")]
        public string? OrderNote { get; set; }

        [Required]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; } = "Cash on Delivery";

        public List<string> PaymentMethods { get; set; } = new()
{
    "Cash on Delivery",
    "Pay Online"
};
    }
}