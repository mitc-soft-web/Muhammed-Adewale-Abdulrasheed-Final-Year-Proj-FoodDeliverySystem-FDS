using System.ComponentModel.DataAnnotations;

namespace FoodDeliverySystem.Models
{
    public class FoodCategory
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public ICollection<FoodItem> FoodItems { get; set; }
            = new List<FoodItem>();
    }
}
