using System.ComponentModel.DataAnnotations;

namespace FoodDeliverySystem.Models
{
    public class Order
    {
        public int Id { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public DateTime? EstimatedDeliveryTime { get; set; }
        
        public DateTime? AcceptedAt { get; set; }

        public DateTime? PreparingAt { get; set; }

        public DateTime? ReadyForPickupAt { get; set; }

        public DateTime? RiderAssignedAt { get; set; }

        public DateTime? OutForDeliveryAt { get; set; }

        public DateTime? DeliveredAt { get; set; }

        public decimal SubTotal { get; set; }

        public decimal DeliveryFee { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Pending";


        // Customer
        [Required]
        public string CustomerId { get; set; } = string.Empty;

        public ApplicationUser? Customer { get; set; }

        // Delivery Details
        [Required]
        public string DeliveryAddress { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        public string? OrderNote { get; set; }

        // Payment
        [Required]
        public string PaymentMethod { get; set; } = "Cash on Delivery";

        public bool IsPaid { get; set; } = false;

        // Rider
        public int? DeliveryRiderId { get; set; }

        public DeliveryRider? DeliveryRider { get; set; }

        public bool RiderAccepted { get; set; } = false;

        // Items
        public ICollection<OrderItem> OrderItems { get; set; }
            = new List<OrderItem>();
    }
}