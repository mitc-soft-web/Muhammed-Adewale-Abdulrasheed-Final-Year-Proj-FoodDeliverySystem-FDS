using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FoodDeliverySystem.ViewModels
{
    public class CreateRiderViewModel
    {
        [Required]
        [Display(Name = "Select User")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Vehicle Type")]
        public string VehicleType { get; set; } = string.Empty;

        public List<SelectListItem> Users { get; set; }
            = new();
    }
}