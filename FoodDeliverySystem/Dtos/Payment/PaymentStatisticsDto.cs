namespace FoodDeliverySystem.DTOs.Payment
{
    public class PaymentStatisticsDto
    {
        public decimal TotalRevenue { get; set; }

        public int TotalPaid { get; set; }

        public int TotalPending { get; set; }

        public int TotalFailed { get; set; }
    }
}