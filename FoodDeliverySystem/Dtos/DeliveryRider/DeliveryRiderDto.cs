namespace FoodDeliverySystem.Dtos.DeliveryRider
{
    public class DeliveryRiderDto
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public string RiderName { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public string VehicleType { get; set; } = string.Empty;

        public string PlateNumber { get; set; } = string.Empty;

        public string DriverLicenseNumber { get; set; } = string.Empty;

        public string? DriverLicenseImage { get; set; }

        public string? ProfileImage { get; set; }

        public string Address { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending";

        public bool IsAvailable { get; set; }

        public DateTime CreatedAt { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public DateTime? LocationUpdatedAt { get; set; }
    }
}