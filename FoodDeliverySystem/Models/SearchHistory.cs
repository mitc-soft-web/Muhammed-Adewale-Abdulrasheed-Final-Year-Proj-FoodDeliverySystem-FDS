using System.ComponentModel.DataAnnotations;

namespace FoodDeliverySystem.Models
{
    public class SearchHistory
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public ApplicationUser? User { get; set; }

        [Required]
        [StringLength(150)]
        public string SearchText { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}