using System.ComponentModel.DataAnnotations;

namespace FoodDeliverySystem.Models
{
    public class DeliveryRider
    {
        public int Id { get; set; }

        // Link to Identity User
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        // Vehicle Information
        [Required]
        public string VehicleType { get; set; }

        [Required]
        public string PlateNumber { get; set; }

        // Driver Licence
        [Required]
        public string DriverLicenseNumber { get; set; }

        public string? DriverLicenseImage { get; set; }

        // Rider Profile
        public string? ProfileImage { get; set; }

        [Required]
        public string Address { get; set; }

        // Admin Approval
        public string Status { get; set; } = "Pending";

        // Rider Availability
        public bool IsAvailable { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public RiderLocation? RiderLocation { get; set; }
    }
}