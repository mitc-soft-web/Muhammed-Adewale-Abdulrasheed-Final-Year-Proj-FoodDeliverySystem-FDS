namespace FoodDeliverySystem.Services
{
    public interface ITrackingService
    {
        Task<TrackingLocationResult> UpdateLocationAsync(
            string userId,
            double latitude,
            double longitude);

        Task<bool> CanTrackOrderAsync(
            string userId,
            int orderId);
    }

    public class TrackingLocationResult
    {
        public bool Success { get; set; }

        public string? ErrorMessage { get; set; }

        public int RiderId { get; set; }

        public List<int> ActiveOrderIds { get; set; } = new();
    }
}