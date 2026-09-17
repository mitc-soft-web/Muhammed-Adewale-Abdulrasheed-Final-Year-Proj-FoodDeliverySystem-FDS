using System.Security.Claims;
using FoodDeliverySystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace FoodDeliverySystem.Hubs
{
    [Authorize]
    public class TrackingHub : Hub
    {
        private readonly ITrackingService _trackingService;

        public TrackingHub(ITrackingService trackingService)
        {
            _trackingService = trackingService;
        }


        // ============================================================
        // RIDER SENDS LOCATION
        // ============================================================

        public async Task UpdateLocation(
            double latitude,
            double longitude)
        {
            var userId = Context.User?
                .FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                throw new HubException("Unauthorized");
            }

            var result =
                await _trackingService.UpdateLocationAsync(
                    userId,
                    latitude,
                    longitude);

            if (!result.Success)
            {
                // Keep the existing tracking behavior.
                // No active delivery simply means the location
                // should not be broadcast.
                if (result.ErrorMessage == "No active deliveries.")
                {
                    Console.WriteLine(
                        "No active deliveries. Location ignored.");

                    return;
                }

                throw new HubException(
                    result.ErrorMessage ?? "Unable to update location.");
            }


            // ========================================================
            // SEND LOCATION TO EVERY CUSTOMER TRACKING THIS RIDER
            // ========================================================

            foreach (var orderId in result.ActiveOrderIds)
            {
                await Clients
                    .Group($"Order_{orderId}")
                    .SendAsync(
                        "ReceiveLocation",
                        result.RiderId,
                        latitude,
                        longitude);

                Console.WriteLine(
                    $"LOCATION SENT Order {orderId} Rider {result.RiderId}");
            }
        }


        // ============================================================
        // CUSTOMER JOINS THEIR ORDER TRACKING ROOM
        // ============================================================

        public async Task JoinOrderGroup(int orderId)
        {
            var userId = Context.User?
                .FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                throw new HubException("Unauthorized");
            }

            var canTrack =
                await _trackingService.CanTrackOrderAsync(
                    userId,
                    orderId);

            if (!canTrack)
            {
                throw new HubException(
                    "You cannot track this order.");
            }

            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                $"Order_{orderId}");

            Console.WriteLine(
                $"Customer {userId} joined Order_{orderId}");
        }
    }
}