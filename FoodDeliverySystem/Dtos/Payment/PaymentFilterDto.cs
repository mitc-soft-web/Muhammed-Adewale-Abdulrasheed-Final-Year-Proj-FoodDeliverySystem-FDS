namespace FoodDeliverySystem.DTOs.Payment
{
    public class PaymentFilterDto
    {
        public string? Search { get; set; }

        public string? Status { get; set; }

        public string? Method { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}