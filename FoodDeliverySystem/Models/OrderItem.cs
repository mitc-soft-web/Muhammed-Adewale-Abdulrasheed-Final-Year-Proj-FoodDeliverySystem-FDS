namespace FoodDeliverySystem.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int Quantity { get; set; }

        // Price at the time the order was placed
        public decimal Price { get; set; }

        // Line Total
        public decimal TotalPrice { get; set; }

        // Order
        public int OrderId { get; set; }

        public Order? Order { get; set; }

        // Food Item
        public int FoodItemId { get; set; }

        public FoodItem? FoodItem { get; set; }
    }
}