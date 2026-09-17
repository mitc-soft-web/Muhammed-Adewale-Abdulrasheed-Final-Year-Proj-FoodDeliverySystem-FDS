namespace FoodDeliverySystem.Dtos.DeliveryRider
{
    public class RiderLocationDto
    {
        public int Id { get; set; }

        public int DeliveryRiderId { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}