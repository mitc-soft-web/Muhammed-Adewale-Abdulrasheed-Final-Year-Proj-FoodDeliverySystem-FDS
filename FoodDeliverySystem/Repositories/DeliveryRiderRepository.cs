using FoodDeliverySystem.Data;
using FoodDeliverySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliverySystem.Repositories
{
    public class DeliveryRiderRepository : IDeliveryRiderRepository
    {
        private readonly ApplicationDbContext _context;

        public DeliveryRiderRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // RIDER MANAGEMENT
        // =========================================================

        public async Task<List<DeliveryRider>> GetAllAsync(
            string? search,
            string? status,
            string? vehicle)
        {
            var query = _context.DeliveryRiders
                .Include(r => r.User)
                .Include(r => r.RiderLocation)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(r =>
                    (r.User != null &&
                     r.User.FullName.Contains(search)) ||

                    (r.User != null &&
                     r.User.Email!.Contains(search)) ||

                    r.PlateNumber.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(r =>
                    r.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(vehicle))
            {
                query = query.Where(r =>
                    r.VehicleType == vehicle);
            }

            return await query
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }


        public async Task<DeliveryRider?> GetByIdAsync(int id)
        {
            return await _context.DeliveryRiders
                .Include(r => r.User)
                .Include(r => r.RiderLocation)
                .FirstOrDefaultAsync(r => r.Id == id);
        }


        public async Task<DeliveryRider?> GetByUserIdAsync(
            string userId)
        {
            return await _context.DeliveryRiders
                .Include(r => r.User)
                .Include(r => r.RiderLocation)
                .FirstOrDefaultAsync(
                    r => r.UserId == userId);
        }


        public async Task AddAsync(
            DeliveryRider rider)
        {
            _context.DeliveryRiders.Add(rider);

            await _context.SaveChangesAsync();
        }


        public async Task UpdateAsync(
            DeliveryRider rider)
        {
            _context.DeliveryRiders.Update(rider);

            await _context.SaveChangesAsync();
        }


        public async Task DeleteAsync(int id)
        {
            var rider =
                await _context.DeliveryRiders
                    .FirstOrDefaultAsync(r => r.Id == id);

            if (rider == null)
            {
                return;
            }

            _context.DeliveryRiders.Remove(rider);

            await _context.SaveChangesAsync();
        }


        // =========================================================
        // RIDER LOCATION
        // =========================================================

        public async Task<RiderLocation?> GetLocationAsync(
            int riderId)
        {
            return await _context.RiderLocations
                .FirstOrDefaultAsync(
                    l => l.DeliveryRiderId == riderId);
        }


        public async Task UpdateLocationAsync(
            int riderId,
            double latitude,
            double longitude)
        {
            var location =
                await _context.RiderLocations
                    .FirstOrDefaultAsync(
                        l => l.DeliveryRiderId == riderId);

            if (location == null)
            {
                location = new RiderLocation
                {
                    DeliveryRiderId = riderId,
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
        }


        // =========================================================
        // RIDER ORDER DETAILS
        // =========================================================

        public async Task<Order?> GetRiderOrderDetailsAsync(
            string userId,
            int orderId)
        {
            return await _context.Orders
                .Include(o => o.Customer)

                .Include(o => o.DeliveryRider)
                    .ThenInclude(r => r!.User)

                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.FoodItem)
                        .ThenInclude(fi => fi.Restaurant)

                .FirstOrDefaultAsync(o =>
                    o.Id == orderId &&
                    o.DeliveryRider != null &&
                    o.DeliveryRider.UserId == userId);
        }


        public async Task<Order?> GetRiderReceiptAsync(
            string userId,
            int orderId)
        {
            return await _context.Orders
                .Include(o => o.Customer)

                .Include(o => o.DeliveryRider)
                    .ThenInclude(r => r!.User)

                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.FoodItem)
                        .ThenInclude(fi => fi.Restaurant)

                .FirstOrDefaultAsync(o =>
                    o.Id == orderId &&
                    o.DeliveryRider != null &&
                    o.DeliveryRider.UserId == userId);
        }


        // =========================================================
        // ASSIGNED ORDERS
        // =========================================================

        public async Task<List<Order>> GetAssignedOrdersAsync(
            string userId)
        {
            return await _context.Orders

                .Include(o => o.Customer)

                .Include(o => o.DeliveryRider)
                    .ThenInclude(r => r!.User)

                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.FoodItem)
                        .ThenInclude(fi => fi.Restaurant)

                .Where(o =>
                    o.DeliveryRider != null &&
                    o.DeliveryRider.UserId == userId &&
                    o.Status != "Delivered" &&
                    o.Status != "Cancelled")

                .OrderByDescending(o => o.OrderDate)

                .ToListAsync();
        }


        // =========================================================
        // DELIVERY HISTORY
        // =========================================================

        public async Task<List<Order>> GetDeliveryHistoryAsync(
            string userId,
            string? search,
            string? paymentMethod,
            string? quickFilter,
            DateTime? fromDate,
            DateTime? toDate)
        {
            var query = _context.Orders

                .Include(o => o.Customer)

                .Include(o => o.DeliveryRider)
                    .ThenInclude(r => r!.User)

                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.FoodItem)
                        .ThenInclude(fi => fi.Restaurant)

                .Where(o =>
                    o.DeliveryRider != null &&
                    o.DeliveryRider.UserId == userId &&
                    o.Status == "Delivered");

            // Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(o =>
                    (o.Customer != null &&
                     o.Customer.FullName.Contains(search)) ||

                    o.Id.ToString().Contains(search));
            }

            // Payment method
            if (!string.IsNullOrWhiteSpace(paymentMethod))
            {
                query = query.Where(o =>
                    o.PaymentMethod == paymentMethod);
            }

            // Date filters
            if (!string.IsNullOrWhiteSpace(quickFilter))
            {
                var today = DateTime.Today;

                switch (quickFilter)
                {
                    case "Today":

                        query = query.Where(o =>
                            o.OrderDate.Date == today);

                        break;


                    case "ThisWeek":

                        var startOfWeek =
                            today.AddDays(
                                -(int)today.DayOfWeek);

                        var endOfWeek =
                            startOfWeek.AddDays(7);

                        query = query.Where(o =>
                            o.OrderDate >= startOfWeek &&
                            o.OrderDate < endOfWeek);

                        break;


                    case "ThisMonth":

                        query = query.Where(o =>
                            o.OrderDate.Month == today.Month &&
                            o.OrderDate.Year == today.Year);

                        break;


                    case "ThisYear":

                        query = query.Where(o =>
                            o.OrderDate.Year == today.Year);

                        break;


                    case "Custom":

                        if (fromDate.HasValue)
                        {
                            query = query.Where(o =>
                                o.OrderDate.Date >=
                                fromDate.Value.Date);
                        }

                        if (toDate.HasValue)
                        {
                            query = query.Where(o =>
                                o.OrderDate.Date <=
                                toDate.Value.Date);
                        }

                        break;
                }
            }

            return await query
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }


        // =========================================================
        // ACCEPT ORDER
        // =========================================================

        public async Task<bool> AcceptOrderAsync(
            string userId,
            int orderId)
        {
            var order =
                await _context.Orders
                    .Include(o => o.DeliveryRider)
                    .FirstOrDefaultAsync(o =>
                        o.Id == orderId &&
                        o.DeliveryRider != null &&
                        o.DeliveryRider.UserId == userId &&
                        o.Status != "Delivered" &&
                        o.Status != "Cancelled");

            if (order == null)
            {
                return false;
            }

            order.RiderAccepted = true;
            order.Status = "Out for Delivery";

            await _context.SaveChangesAsync();

            return true;
        }


        // =========================================================
        // COMPLETE ORDER
        // =========================================================

        public async Task<bool> CompleteOrderAsync(
            string userId,
            int orderId)
        {
            var order =
                await _context.Orders
                    .Include(o => o.DeliveryRider)
                    .FirstOrDefaultAsync(o =>
                        o.Id == orderId &&
                        o.DeliveryRider != null &&
                        o.DeliveryRider.UserId == userId &&
                        o.Status != "Delivered" &&
                        o.Status != "Cancelled");

            if (order == null)
            {
                return false;
            }

            if (!order.RiderAccepted)
            {
                return false;
            }

            order.Status = "Delivered";

            await _context.SaveChangesAsync();

            return true;
        }


        // =========================================================
        // RIDER AVAILABILITY
        // =========================================================

        public async Task ToggleAvailabilityAsync(
            string userId)
        {
            var rider =
                await _context.DeliveryRiders
                    .FirstOrDefaultAsync(
                        r => r.UserId == userId);

            if (rider == null)
            {
                return;
            }

            rider.IsAvailable = !rider.IsAvailable;

            await _context.SaveChangesAsync();
        }
    }
}