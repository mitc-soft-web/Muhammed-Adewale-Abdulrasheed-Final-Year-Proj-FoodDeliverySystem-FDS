namespace FoodDeliverySystem.Models
{
    public class RiderLocation
    {
        public int Id { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public int DeliveryRiderId { get; set; }

        public DeliveryRider DeliveryRider { get; set; }
    }
}