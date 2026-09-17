namespace FoodDeliverySystem.Models
{
    public class Payment
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        public string PaymentMethod { get; set; }

        public string PaymentStatus { get; set; }

        public string? PaymentReference { get; set; }

        public DateTime ExpiresAt { get; set; }

        public bool IsExpired { get; set; } = false;

        public int OrderId { get; set; }

        public Order? Order { get; set; }
    }
}