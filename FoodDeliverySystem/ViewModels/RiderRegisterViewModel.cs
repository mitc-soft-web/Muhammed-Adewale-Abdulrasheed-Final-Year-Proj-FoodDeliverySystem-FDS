using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace FoodDeliverySystem.ViewModels
{
    public class RiderRegisterViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Vehicle Type")]
        public string VehicleType { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Plate Number")]
        public string PlateNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Driver Licence Number")]
        public string DriverLicenseNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Driver Licence")]
        public IFormFile DriverLicenseFile { get; set; }

        [Required]
        [Display(Name = "Home Address")]
        public string Address { get; set; } = string.Empty;
    }
}