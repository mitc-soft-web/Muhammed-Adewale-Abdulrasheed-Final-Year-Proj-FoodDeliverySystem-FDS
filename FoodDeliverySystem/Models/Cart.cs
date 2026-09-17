using System.ComponentModel.DataAnnotations;

namespace FoodDeliverySystem.Models
{
    public class Cart
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public ApplicationUser? User { get; set; }

        public ICollection<CartItem> CartItems { get; set; }
            = new List<CartItem>();
    }
}
