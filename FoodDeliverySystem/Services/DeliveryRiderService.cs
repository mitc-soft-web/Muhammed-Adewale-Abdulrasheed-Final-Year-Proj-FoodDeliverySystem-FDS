using DocumentFormat.OpenXml.InkML;
using FoodDeliverySystem.Models;
using FoodDeliverySystem.Repositories;
using FoodDeliverySystem.ViewModels;

namespace FoodDeliverySystem.Services
{
    public class DeliveryRiderService : IDeliveryRiderService
    {
        private readonly IDeliveryRiderRepository _repository;

        public DeliveryRiderService(
            IDeliveryRiderRepository repository)
        {
            _repository = repository;
        }


        // =========================================================
        // ADMIN RIDER MANAGEMENT
        // =========================================================

        public async Task<List<DeliveryRider>> GetAllAsync(
        string? search,
        string? status,
        string? vehicle)
        {
            return await _repository.GetAllAsync(
                search,
                status,
                vehicle);
        }


        public async Task<DeliveryRider?> GetByIdAsync(
            int id)
        {
            return await _repository.GetByIdAsync(id);
        }


        public async Task<DeliveryRider?> GetByUserIdAsync(
            string userId)
        {
            return await _repository.GetByUserIdAsync(userId);
        }


        public async Task AddAsync(
            DeliveryRider rider)
        {
            await _repository.AddAsync(rider);
        }


        public async Task UpdateAsync(
            DeliveryRider rider)
        {
            await _repository.UpdateAsync(rider);
        }


        public async Task DeleteAsync(
            int id)
        {
            await _repository.DeleteAsync(id);
        }


        public async Task<bool> ApproveAsync(
            int id)
        {
            var rider =
                await _repository.GetByIdAsync(id);

            if (rider == null)
            {
                return false;
            }

            rider.Status = "Approved";

            await _repository.UpdateAsync(rider);

            return true;
        }


        public async Task<bool> RejectAsync(
            int id)
        {
            var rider =
                await _repository.GetByIdAsync(id);

            if (rider == null)
            {
                return false;
            }

            rider.Status = "Rejected";

            await _repository.UpdateAsync(rider);

            return true;
        }


        // =========================================================
        // RIDER LOCATION
        // =========================================================

        public async Task<RiderLocation?> GetLocationAsync(
            int riderId)
        {
            return await _repository.GetLocationAsync(
                riderId);
        }


        public async Task UpdateLocationAsync(
            int riderId,
            double latitude,
            double longitude)
        {
            await _repository.UpdateLocationAsync(
                riderId,
                latitude,
                longitude);
        }


        // =========================================================
        // RIDER AVAILABILITY
        // =========================================================

        public async Task ToggleAvailabilityAsync(
            string userId)
        {
            await _repository.ToggleAvailabilityAsync(
                userId);
        }


        // =========================================================
        // RIDER ORDER DETAILS
        // =========================================================

        public async Task<Order?> GetRiderOrderDetailsAsync(
            string userId,
            int orderId)
        {
            return await _repository
                .GetRiderOrderDetailsAsync(
                    userId,
                    orderId);
        }


        public async Task<Order?> GetRiderReceiptAsync(
            string userId,
            int orderId)
        {
            return await _repository
                .GetRiderReceiptAsync(
                    userId,
                    orderId);
        }


        // =========================================================
        // ASSIGNED ORDERS
        // =========================================================

        public async Task<List<Order>> GetAssignedOrdersAsync(
            string userId)
        {
            return await _repository
                .GetAssignedOrdersAsync(userId);
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
            return await _repository
                .GetDeliveryHistoryAsync(
                    userId,
                    search,
                    paymentMethod,
                    quickFilter,
                    fromDate,
                    toDate);
        }


        public async Task<RiderDeliveryHistoryResult>
GetDeliveryHistorySummaryAsync(
    string userId,
    string? search,
    string? paymentMethod,
    string? quickFilter,
    DateTime? fromDate,
    DateTime? toDate)
        {
            var deliveries =
                await _repository.GetDeliveryHistoryAsync(
                    userId,
                    search,
                    paymentMethod,
                    quickFilter,
                    fromDate,
                    toDate);

            var cashOrders =
                deliveries.Count(o =>
                    o.PaymentMethod == "Cash on Delivery");

            var onlineOrders =
                deliveries.Count(o =>
                    o.PaymentMethod == "Pay Online");

            return new RiderDeliveryHistoryResult
            {
                Deliveries = deliveries,

                TotalDeliveries =
                    deliveries.Count,

                CashOrders =
                    cashOrders,

                OnlineOrders =
                    onlineOrders,

                TotalEarnings =
                    deliveries.Sum(o =>
                        o.TotalAmount)
            };
        }


        // =========================================================
        // ACCEPT ORDER
        // =========================================================

        public async Task<bool> AcceptOrderAsync(
            string userId,
            int orderId)
        {
            return await _repository
                .AcceptOrderAsync(
                    userId,
                    orderId);
        }


        // =========================================================
        // COMPLETE ORDER
        // =========================================================

        public async Task<bool> CompleteOrderAsync(
            string userId,
            int orderId)
        {
            return await _repository
                .CompleteOrderAsync(
                    userId,
                    orderId);
        }


        // =========================================================
        // REGISTRATION
        // =========================================================

        public async Task<bool> RegisterRiderAsync(
        RiderRegisterViewModel model,
        string userId,
        string? licenseFileName)
        {
            var rider = new DeliveryRider
            {
                UserId = userId,
                VehicleType = model.VehicleType,
                PlateNumber = model.PlateNumber,
                DriverLicenseNumber = model.DriverLicenseNumber,
                DriverLicenseImage = licenseFileName,
                Address = model.Address,
                Status = "Pending",
                IsAvailable = false
            };

            await _repository.AddAsync(rider);

            return true;
        }


        // =========================================================
        // DASHBOARD
        // =========================================================

        public async Task<RiderDashboardResult?> GetDashboardAsync(
    string userId)
        {
            var rider =
                await _repository.GetByUserIdAsync(userId);

            if (rider == null)
            {
                return null;
            }

            var activeOrders =
                await _repository.GetAssignedOrdersAsync(userId);

            var deliveredOrders =
                await _repository.GetDeliveryHistoryAsync(
                    userId,
                    null,
                    null,
                    null,
                    null,
                    null);

            var today = DateTime.Today;

            var cashOrders =
                deliveredOrders
                    .Where(o =>
                        o.PaymentMethod == "Cash on Delivery")
                    .ToList();

            var onlineOrders =
                deliveredOrders
                    .Where(o =>
                        o.PaymentMethod == "Pay Online")
                    .ToList();

            var todayDeliveries =
                deliveredOrders
                    .Where(o =>
                        o.OrderDate.Date == today)
                    .ToList();

            return new RiderDashboardResult
            {
                Rider = rider,

                AssignedOrdersCount =
                    activeOrders.Count,

                NewOrdersCount =
                    activeOrders.Count(o =>
                        !o.RiderAccepted),

                CompletedDeliveriesCount =
                    deliveredOrders.Count,

                ActiveOrders =
                    activeOrders,

                DeliveredOrders =
                    deliveredOrders,

                TotalDeliveries =
                    deliveredOrders.Count,

                CashOrders =
                    cashOrders.Count,

                OnlineOrders =
                    onlineOrders.Count,

                CashCollected =
                    cashOrders.Sum(o =>
                        o.TotalAmount),

                OnlineCollected =
                    onlineOrders.Sum(o =>
                        o.TotalAmount),

                TotalEarnings =
                    deliveredOrders.Sum(o =>
                        o.TotalAmount),

                TodayDeliveries =
                    todayDeliveries.Count,

                TodayEarnings =
                    todayDeliveries.Sum(o =>
                        o.TotalAmount),

                RecentDeliveries =
                    deliveredOrders
                        .Take(5)
                        .ToList()
            };
        }
    }
}