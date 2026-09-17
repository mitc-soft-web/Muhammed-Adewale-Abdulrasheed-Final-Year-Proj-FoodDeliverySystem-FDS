using System.ComponentModel.DataAnnotations;

namespace FoodDeliverySystem.Models
{
    public class Restaurant
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? Logo { get; set; }

        public string? CoverImage { get; set; }

        public bool IsOpen { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // We'll connect restaurant owners later
        public string? OwnerId { get; set; }

        public ApplicationUser? Owner { get; set; }

        public ICollection<FoodItem> FoodItems { get; set; }
            = new List<FoodItem>();
    }
}