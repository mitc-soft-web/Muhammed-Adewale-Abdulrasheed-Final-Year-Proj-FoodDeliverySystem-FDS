using System.ComponentModel.DataAnnotations;

namespace FoodDeliverySystem.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        [Required]
        public int CartId { get; set; }

        public Cart? Cart { get; set; }

        [Required]
        public int FoodItemId { get; set; }

        public FoodItem? FoodItem { get; set; }

        [Required]
        public int Quantity { get; set; } = 1;
    }
}
