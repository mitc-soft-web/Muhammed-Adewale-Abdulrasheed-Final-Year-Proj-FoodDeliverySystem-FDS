namespace FoodDeliverySystem.DTOs.Order
{
    public class OrderDto
    {
        public int Id { get; set; }

        // =========================
        // CUSTOMER
        // =========================

        public string CustomerId { get; set; } = string.Empty;

        public string? CustomerName { get; set; }

        public string? CustomerEmail { get; set; }

        public string? CustomerPhone { get; set; }

        // =========================
        // DELIVERY
        // =========================

        public string DeliveryAddress { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string? OrderNote { get; set; }

        // =========================
        // AMOUNTS
        // =========================


        public string? DeliveryRiderEmail { get; set; }

        public string? DeliveryRiderVehicle { get; set; }

        public string? DeliveryRiderPlateNumber { get; set; }

        public string? DeliveryRiderAddress { get; set; }

        public string? DeliveryRiderStatus { get; set; }

        public bool DeliveryRiderIsAvailable { get; set; }

        public decimal SubTotal { get; set; }

        public decimal DeliveryFee { get; set; }

        public decimal TotalAmount { get; set; }

        // =========================
        // PAYMENT
        // =========================

        public string PaymentMethod { get; set; } = string.Empty;

        public bool IsPaid { get; set; }

        // =========================
        // ORDER
        // =========================

        public string Status { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; }

        public DateTime CreatedAt { get; set; }

        // =========================
        // RIDER
        // =========================

        public int? DeliveryRiderId { get; set; }

        public string? DeliveryRiderName { get; set; }

        public string? DeliveryRiderPhone { get; set; }

        public string? VehicleType { get; set; }

        public string? PlateNumber { get; set; }

        public string? RiderStatus { get; set; }

        public bool RiderAccepted { get; set; }

        // =========================
        // ITEMS
        // =========================

        public List<OrderItemDto> Items { get; set; }
            = new();
    }


    public class OrderItemDto
    {
        public int Id { get; set; }

        public int FoodItemId { get; set; }

        public string? FoodItemName { get; set; }

        public decimal Price { get; set; }

        public decimal TotalPrice { get; set; }

        public int Quantity { get; set; }

        public string? FoodImageUrl { get; set; }

        public string? RestaurantName { get; set; }

        public string? RestaurantAddress { get; set; }

        public string? RestaurantPhoneNumber { get; set; }

        public string? RestaurantDescription { get; set; }

        public string? RestaurantLogo { get; set; }

        public string? RestaurantCoverImage { get; set; }

        public string? CategoryName { get; set; }
    }
}