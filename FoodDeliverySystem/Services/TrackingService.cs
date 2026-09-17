using FoodDeliverySystem.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliverySystem.Services
{
    public class TrackingService : ITrackingService
    {
        private readonly ApplicationDbContext _context;

        public TrackingService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // RIDER LOCATION
        // ============================================================

        public async Task<TrackingLocationResult> UpdateLocationAsync(
            string userId,
            double latitude,
            double longitude)
        {
            // Validate latitude
            if (latitude < -90 || latitude > 90)
            {
                return new TrackingLocationResult
                {
                    Success = false,
                    ErrorMessage = "Invalid latitude."
                };
            }

            // Validate longitude
            if (longitude < -180 || longitude > 180)
            {
                return new TrackingLocationResult
                {
                    Success = false,
                    ErrorMessage = "Invalid longitude."
                };
            }

            // Find rider profile
            var rider = await _context.DeliveryRiders
                .FirstOrDefaultAsync(r => r.UserId == userId);

            if (rider == null)
            {
                return new TrackingLocationResult
                {
                    Success = false,
                    ErrorMessage = "Rider profile not found."
                };
            }

            // Find active deliveries belonging to this rider
            var activeOrderIds = await _context.Orders
                .Where(o =>
                    o.DeliveryRiderId == rider.Id &&
                    o.RiderAccepted &&
                    o.Status == "Out for Delivery")
                .Select(o => o.Id)
                .ToListAsync();

            // No active delivery
            if (!activeOrderIds.Any())
            {
                return new TrackingLocationResult
                {
                    Success = false,
                    ErrorMessage = "No active deliveries."
                };
            }

            // Find existing rider location
            var location = await _context.RiderLocations
                .FirstOrDefaultAsync(l =>
                    l.DeliveryRiderId == rider.Id);

            if (location == null)
            {
                location = new Models.RiderLocation
                {
                    DeliveryRiderId = rider.Id,
                    Latitude = latitude,
                    Longitude = longitude,
                    UpdatedAt = DateTime.Now
                };

                _context.RiderLocations.Add(location);
            }
            else
            {
                location.Latitude = latitude;
                location.Longitude = longitude;
                location.UpdatedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            return new TrackingLocationResult
            {
                Success = true,
                RiderId = rider.Id,
                ActiveOrderIds = activeOrderIds
            };
        }


        // ============================================================
        // CUSTOMER ORDER TRACKING SECURITY
        // ============================================================

        public async Task<bool> CanTrackOrderAsync(
            string userId,
            int orderId)
        {
            return await _context.Orders
                .AnyAsync(o =>
                    o.Id == orderId &&
                    o.CustomerId == userId);
        }
    }
}