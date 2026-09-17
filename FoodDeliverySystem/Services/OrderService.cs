using FoodDeliverySystem.DTOs.Order;
using FoodDeliverySystem.Models;
using FoodDeliverySystem.Repositories;

namespace FoodDeliverySystem.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(
            IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        // =========================================================
        // CUSTOMER ORDERS
        // =========================================================

        public async Task<List<OrderDto>> GetCustomerOrdersAsync(
            string customerId)
        {
            var orders =
                await _orderRepository.GetCustomerOrdersAsync(
                    customerId);

            return orders
                .Select(MapToDto)
                .ToList();
        }

        // =========================================================
        // ADMIN ORDERS
        // =========================================================

        public async Task<OrderListDto> GetAllOrdersAsync(
            string? search,
            string? status,
            string? dateFilter,
            int page,
            int pageSize)
        {
            var orders =
                await _orderRepository.GetAllOrdersAsync(
                    search,
                    status,
                    dateFilter,
                    page,
                    pageSize);

            var totalOrders =
                await _orderRepository.GetTotalOrdersCountAsync(
                    search,
                    status,
                    dateFilter);

            return new OrderListDto
            {
                Orders = orders
                    .Select(MapToDto)
                    .ToList(),

                TotalOrders = totalOrders,

                PendingOrders = orders.Count(o =>
                    o.Status == "Pending"),

                PreparingOrders = orders.Count(o =>
                    o.Status == "Preparing"),

                DeliveryOrders = orders.Count(o =>
                    o.Status == "Out for Delivery"),

                DeliveredOrders = orders.Count(o =>
                    o.Status == "Delivered"),

                TotalPages = (int)Math.Ceiling(
                    totalOrders / (double)pageSize)
            };
        }

        // =========================================================
        // ADMIN ORDER DETAILS
        // =========================================================

        public async Task<OrderDto?> GetAdminOrderDetailsAsync(
            int orderId)
        {
            var order =
                await _orderRepository.GetAdminOrderDetailsAsync(
                    orderId);

            if (order == null)
                return null;

            return MapToDto(order);
        }

        // =========================================================
        // CUSTOMER ORDER DETAILS
        // =========================================================

        public async Task<OrderDto?> GetCustomerOrderDetailsAsync(
            int orderId,
            string customerId)
        {
            var order =
                await _orderRepository.GetCustomerOrderDetailsAsync(
                    orderId,
                    customerId);

            if (order == null)
                return null;

            return MapToDto(order);
        }

        // =========================================================
        // CUSTOMER TRACKING
        // =========================================================

        public async Task<OrderDto?> GetCustomerTrackingOrderAsync(
            int orderId,
            string customerId)
        {
            var order =
                await _orderRepository.GetCustomerTrackingOrderAsync(
                    orderId,
                    customerId);

            if (order == null)
                return null;

            return MapToDto(order);
        }

        // =========================================================
        // AVAILABLE RIDERS
        // =========================================================

        public async Task<List<DeliveryRider>> GetAvailableRidersAsync()
        {
            return await _orderRepository
                .GetAvailableRidersAsync();
        }

        // =========================================================
        // UPDATE STATUS
        // =========================================================

        public async Task<bool> UpdateStatusAsync(
            int orderId,
            string status)
        {
            var order =
                await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
                return false;

            order.Status = status;

            await _orderRepository.UpdateAsync(order);

            return true;
        }

        // =========================================================
        // UPDATE ORDER
        // =========================================================

        public async Task<(bool Success, string? Error)> UpdateOrderAsync(
            int orderId,
            string status,
            int? deliveryRiderId)
        {
            var order =
                await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
            {
                return (
                    false,
                    "Order not found.");
            }

            // Preserve existing business rule.
            if (order.Status == "Delivered" ||
                order.Status == "Cancelled" ||
                order.Status == "Payment Failed")
            {
                return (
                    false,
                    "This order has been closed and can no longer be modified.");
            }

            order.Status = status;
            order.DeliveryRiderId = deliveryRiderId;

            await _orderRepository.UpdateAsync(order);

            return (true, null);
        }

        // =========================================================
        // EXPORT
        // =========================================================

        public async Task<List<Order>> GetOrdersForExportAsync()
        {
            return await _orderRepository
                .GetOrdersForExportAsync();
        }

        // =========================================================
        // EXPIRE UNPAID PAYMENTS
        // =========================================================

        public async Task ExpireUnpaidPaymentsAsync()
        {
            await _orderRepository
                .ExpireUnpaidPaymentsAsync();
        }

        // =========================================================
        // ENTITY -> DTO
        // =========================================================

        private static OrderDto MapToDto(Order order)
        {
            return new OrderDto
            {
                // -------------------------------------------------
                // ORDER
                // -------------------------------------------------

                Id = order.Id,

                CustomerId = order.CustomerId,

                DeliveryAddress =
                    order.DeliveryAddress,

                PhoneNumber =
                    order.PhoneNumber,

                SubTotal =
                    order.SubTotal,

                DeliveryFee =
                    order.DeliveryFee,

                TotalAmount =
                    order.TotalAmount,

                PaymentMethod =
                    order.PaymentMethod,

                IsPaid =
                    order.IsPaid,

                Status =
                    order.Status,

                OrderDate =
                    order.OrderDate,

                CreatedAt =
                    order.CreatedAt,

                OrderNote =
                    order.OrderNote,

                RiderAccepted =
                    order.RiderAccepted,

                DeliveryRiderId =
                    order.DeliveryRiderId,

                // -------------------------------------------------
                // CUSTOMER
                // -------------------------------------------------

                CustomerName =
                    order.Customer?.FullName,

                CustomerPhone =
                    order.Customer?.PhoneNumber,

                CustomerEmail =
                    order.Customer?.Email,

                // -------------------------------------------------
                // RIDER
                // -------------------------------------------------

                DeliveryRiderName =
                    order.DeliveryRider?.User?.FullName,

                DeliveryRiderEmail =
                    order.DeliveryRider?.User?.Email,

                DeliveryRiderVehicle =
                    order.DeliveryRider?.VehicleType,

                DeliveryRiderPlateNumber =
                    order.DeliveryRider?.PlateNumber,

                DeliveryRiderAddress =
                    order.DeliveryRider?.Address,

                DeliveryRiderStatus =
                    order.DeliveryRider?.Status,

                DeliveryRiderIsAvailable =
                    order.DeliveryRider?.IsAvailable ?? false,

                // -------------------------------------------------
                // ORDER ITEMS
                // -------------------------------------------------

                Items = order.OrderItems?
                    .Select(item => new OrderItemDto
                    {
                        Id =
                            item.Id,

                        FoodItemId =
                            item.FoodItemId,

                        FoodItemName =
                            item.FoodItem?.Name,

                        // IMPORTANT:
                        // Keep the actual OrderItem price.
                        Price =
                            item.Price,

                        // IMPORTANT:
                        // Keep the actual OrderItem total.
                        TotalPrice =
                            item.TotalPrice,

                        Quantity =
                            item.Quantity,

                        FoodImageUrl =
                            item.FoodItem?.ImageUrl,

                        // -------------------------------------------------
                        // RESTAURANT
                        // -------------------------------------------------

                        RestaurantName =
                            item.FoodItem?.Restaurant?.Name,

                        RestaurantAddress =
                            item.FoodItem?.Restaurant?.Address,

                        RestaurantPhoneNumber =
                            item.FoodItem?.Restaurant?.PhoneNumber,

                        RestaurantDescription =
                            item.FoodItem?.Restaurant?.Description,

                        RestaurantLogo =
                            item.FoodItem?.Restaurant?.Logo,

                        RestaurantCoverImage =
                            item.FoodItem?.Restaurant?.CoverImage,

                        // -------------------------------------------------
                        // CATEGORY
                        // -------------------------------------------------

                        CategoryName =
                            item.FoodItem?.FoodCategory?.Name
                    })
                    .ToList()
                    ?? new List<OrderItemDto>()
            };
        }
    }
}