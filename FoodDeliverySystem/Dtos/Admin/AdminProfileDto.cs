namespace FoodDeliverySystem.DTOs.Admin
{
    public class AdminProfileDto
    {
        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? Gender { get; set; }
    }
}