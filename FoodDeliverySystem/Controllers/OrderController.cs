using ClosedXML.Excel;
using FoodDeliverySystem.DTOs.Order;
using FoodDeliverySystem.Models;
using FoodDeliverySystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliverySystem.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOrderService _orderService;

        public OrderController(
            UserManager<ApplicationUser> userManager,
            IOrderService orderService)
        {
            _userManager = userManager;
            _orderService = orderService;
        }

        // =========================================================
        // CUSTOMER - ORDER HISTORY
        // =========================================================

        public async Task<IActionResult> MyOrders()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            await _orderService.ExpireUnpaidPaymentsAsync();

            var orderDtos =
                await _orderService.GetCustomerOrdersAsync(user.Id);

            var orders = orderDtos
                .Where(dto => dto != null)
                .Select(MapDtoToOrder)
                .ToList();

            return View(orders);
        }

        // =========================================================
        // ADMIN - ALL ORDERS
        // =========================================================

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(
            string? search,
            string? status,
            string? dateFilter,
            int page = 1)
        {
            const int pageSize = 10;

            if (page < 1)
            {
                page = 1;
            }

            var result =
                await _orderService.GetAllOrdersAsync(
                    search,
                    status,
                    dateFilter,
                    page,
                    pageSize);

            var orders = result.Orders
                .Where(dto => dto != null)
                .Select(MapDtoToOrder)
                .ToList();

            ViewBag.TotalOrders = result.TotalOrders;
            ViewBag.PendingOrders = result.PendingOrders;
            ViewBag.PreparingOrders = result.PreparingOrders;
            ViewBag.DeliveryOrders = result.DeliveryOrders;
            ViewBag.DeliveredOrders = result.DeliveredOrders;

            ViewBag.Search = search;
            ViewBag.Status = status;
            ViewBag.DateFilter = dateFilter;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = result.TotalPages;

            return View(orders);
        }

        // =========================================================
        // ADMIN - ORDER DETAILS
        // =========================================================

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDetails(int id)
        {
            var orderDto =
                await _orderService.GetAdminOrderDetailsAsync(id);

            if (orderDto == null)
            {
                return NotFound();
            }

            // IMPORTANT:
            // Existing AdminDetails.cshtml expects Models.Order.
            // Do not change the existing view.
            var order = MapDtoToOrder(orderDto);

            ViewBag.Riders =
                await _orderService.GetAvailableRidersAsync();

            return View(order);
        }

        // =========================================================
        // ADMIN - UPDATE STATUS
        // =========================================================

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(
            int id,
            string status)
        {
            var success =
                await _orderService.UpdateStatusAsync(
                    id,
                    status);

            if (!success)
            {
                return NotFound();
            }

            return RedirectToAction(
                nameof(AdminDetails),
                new { id });
        }

        // =========================================================
        // ADMIN - EDIT ORDER
        // =========================================================

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Order order)
        {
            var result =
                await _orderService.UpdateOrderAsync(
                    order.Id,
                    order.Status,
                    order.DeliveryRiderId);

            if (!result.Success)
            {
                if (result.Error == "Order not found.")
                {
                    return NotFound();
                }

                TempData["Error"] = result.Error;

                return RedirectToAction(
                    nameof(AdminDetails),
                    new { id = order.Id });
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================================================
        // CUSTOMER - ORDER DETAILS
        // =========================================================

        public async Task<IActionResult> Details(int id)
        {
            var user =
                await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            var orderDto =
                await _orderService.GetCustomerOrderDetailsAsync(
                    id,
                    user.Id);

            if (orderDto == null)
            {
                return NotFound();
            }

            // Existing Details.cshtml expects Models.Order.
            var order = MapDtoToOrder(orderDto);

            return View(order);
        }

        // =========================================================
        // CUSTOMER - LIVE TRACKING
        // =========================================================

        public async Task<IActionResult> TrackOrder(int id)
        {
            var user =
                await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            var orderDto =
                await _orderService.GetCustomerTrackingOrderAsync(
                    id,
                    user.Id);

            if (orderDto == null)
            {
                return NotFound();
            }

            if (orderDto.DeliveryRiderId == null)
            {
                TempData["Error"] =
                    "This order has not been assigned to a rider yet.";

                return RedirectToAction(nameof(MyOrders));
            }

            if (orderDto.Status != "Out for Delivery")
            {
                TempData["Error"] =
                    "Live tracking is only available when the order is out for delivery.";

                return RedirectToAction(nameof(MyOrders));
            }

            // Existing TrackOrder.cshtml expects Models.Order.
            var order = MapDtoToOrder(orderDto);

            return View(order);
        }

        // =========================================================
        // ADMIN - EXPORT ORDERS
        // =========================================================

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportOrders()
        {
            var orders =
                await _orderService.GetOrdersForExportAsync();

            using var workbook = new XLWorkbook();

            var worksheet =
                workbook.Worksheets.Add("Orders");

            var logoPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "images",
                "logo-light.png");

            if (System.IO.File.Exists(logoPath))
            {
                worksheet.AddPicture(logoPath)
                    .MoveTo(worksheet.Cell("A1"))
                    .Scale(0.28);
            }

            worksheet.Cell(2, 3).Value = "FOOD XPRESS";

            worksheet.Cell(3, 3).Value = "Orders Report";

            worksheet.Cell(4, 3).Value =
                $"Export Date: {DateTime.Now:dd MMM yyyy HH:mm}";

            worksheet.Range("C2:G2").Merge();
            worksheet.Range("C3:G3").Merge();
            worksheet.Range("C4:G4").Merge();

            worksheet.Range("C2:G4")
                .Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;

            worksheet.Range("C2:G4")
                .Style.Alignment.Vertical =
                XLAlignmentVerticalValues.Center;

            worksheet.Cell(6, 1).Value = "Order ID";
            worksheet.Cell(6, 2).Value = "Customer";
            worksheet.Cell(6, 3).Value = "Order Date";
            worksheet.Cell(6, 4).Value = "Amount";
            worksheet.Cell(6, 5).Value = "Payment";
            worksheet.Cell(6, 6).Value = "Status";
            worksheet.Cell(6, 7).Value = "Assigned Rider";

            var header =
                worksheet.Range("A6:G6");

            header.Style.Font.Bold = true;
            header.Style.Font.FontColor = XLColor.White;

            header.Style.Fill.BackgroundColor =
                XLColor.FromHtml("#ff6b35");

            header.Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;

            header.Style.Alignment.Vertical =
                XLAlignmentVerticalValues.Center;

            worksheet.Row(1).Height = 28;

            worksheet.Cell(1, 1)
                .Style.Font.Bold = true;

            worksheet.Cell(1, 1)
                .Style.Font.FontSize = 22;

            worksheet.Cell(1, 1)
                .Style.Font.FontColor =
                XLColor.FromHtml("#ff6b35");

            worksheet.Cell(2, 1)
                .Style.Font.Bold = true;

            worksheet.Cell(2, 1)
                .Style.Font.FontSize = 16;

            worksheet.Cell(3, 1)
                .Style.Font.Italic = true;

            int row = 7;

            foreach (var order in orders)
            {
                worksheet.Cell(row, 1).Value =
                    order.Id;

                worksheet.Cell(row, 2).Value =
                    order.Customer?.FullName;

                worksheet.Cell(row, 3).Value =
                    order.OrderDate.ToString(
                        "dd MMM yyyy HH:mm");

                worksheet.Cell(row, 4).Value =
                    order.TotalAmount;

                worksheet.Cell(row, 5).Value =
                    order.PaymentMethod;

                worksheet.Cell(row, 6).Value =
                    order.Status;

                worksheet.Cell(row, 7).Value =
                    order.DeliveryRider != null
                        ? order.DeliveryRider.User?.FullName
                        : "Not Assigned";

                row++;
            }

            worksheet.Column(4)
                .Style.NumberFormat.Format =
                "₦#,##0";

            worksheet.Columns()
                .AdjustToContents();

            worksheet.Column(2).Width = 28;
            worksheet.Column(7).Width = 25;

            worksheet.Column(1)
                .Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;

            worksheet.Column(3)
                .Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;

            worksheet.Column(5)
                .Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;

            worksheet.Column(6)
                .Style.Alignment.Horizontal =
                XLAlignmentHorizontalValues.Center;

            worksheet.SheetView.FreezeRows(1);

            worksheet.Range($"A6:G{row - 1}")
                .SetAutoFilter();

            worksheet.Cell(row + 2, 3).Value =
                "Total Orders:";

            worksheet.Cell(row + 2, 4).Value =
                orders.Count;

            worksheet.Cell(row + 3, 3).Value =
                "Total Revenue:";

            worksheet.Cell(row + 3, 4).Value =
                orders.Sum(o => o.TotalAmount);

            worksheet.Range($"C{row + 2}:D{row + 3}")
                .Style.Font.Bold = true;

            worksheet.Cell(row + 3, 4)
                .Style.NumberFormat.Format =
                "₦#,##0";

            using var stream = new MemoryStream();

            workbook.SaveAs(stream);

            stream.Position = 0;

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"FoodXpressOrders_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
        }

        // =========================================================
        // DTO -> EXISTING MODEL
        // =========================================================

        private static Order MapDtoToOrder(OrderDto dto)
        {
            return new Order
            {
                Id = dto.Id,

                CustomerId = dto.CustomerId,

                DeliveryAddress =
                    dto.DeliveryAddress ?? string.Empty,

                PhoneNumber =
                    dto.PhoneNumber ?? string.Empty,

                SubTotal = dto.SubTotal,

                DeliveryFee = dto.DeliveryFee,

                TotalAmount = dto.TotalAmount,

                PaymentMethod =
                    dto.PaymentMethod ?? string.Empty,

                IsPaid = dto.IsPaid,

                Status =
                    dto.Status ?? string.Empty,

                OrderDate = dto.OrderDate,

                CreatedAt = dto.CreatedAt,

                DeliveryRiderId =
                    dto.DeliveryRiderId,

                OrderNote = dto.OrderNote,

                RiderAccepted =
                    dto.RiderAccepted,

                // =================================================
                // CUSTOMER
                // =================================================

                Customer = new ApplicationUser
                {
                    Id = dto.CustomerId,

                    FullName =
                        dto.CustomerName ?? string.Empty,

                    Email =
                        dto.CustomerEmail,

                    PhoneNumber =
                        dto.CustomerPhone ?? string.Empty
                },

                // =================================================
                // DELIVERY RIDER
                // =================================================

                DeliveryRider =
                    dto.DeliveryRiderId.HasValue
                        ? new DeliveryRider
                        {
                            Id =
                                dto.DeliveryRiderId.Value,

                            VehicleType =
                                dto.DeliveryRiderVehicle
                                ?? string.Empty,

                            PlateNumber =
                                dto.DeliveryRiderPlateNumber
                                ?? string.Empty,

                            Address =
                                dto.DeliveryRiderAddress
                                ?? string.Empty,

                            Status =
                                dto.DeliveryRiderStatus
                                ?? string.Empty,

                            IsAvailable =
                                dto.DeliveryRiderIsAvailable,

                            User = new ApplicationUser
                            {
                                FullName =
                                    dto.DeliveryRiderName
                                    ?? "Unknown Rider",

                                Email =
                                    dto.DeliveryRiderEmail
                            }
                        }
                        : null,

                // =================================================
                // ORDER ITEMS
                // =================================================

                OrderItems =
                    dto.Items?
                        .Select(item => new OrderItem
                        {
                            Id = item.Id,

                            OrderId = dto.Id,

                            FoodItemId =
                                item.FoodItemId,

                            Quantity =
                                item.Quantity,

                            // IMPORTANT:
                            // Preserve the actual item price
                            // from the DTO.
                            Price =
                                item.Price,

                            TotalPrice =
                                item.TotalPrice,

                            FoodItem = new FoodItem
                            {
                                Id =
                                    item.FoodItemId,

                                Name =
                                    item.FoodItemName
                                    ?? string.Empty,

                                Price =
                                    item.Price,

                                ImageUrl =
                                    item.FoodImageUrl,

                                Restaurant =
                                    new Restaurant
                                    {
                                        Name =
                                            item.RestaurantName
                                            ?? string.Empty,

                                        Address =
                                            item.RestaurantAddress
                                            ?? string.Empty,

                                        PhoneNumber =
                                            item.RestaurantPhoneNumber
                                            ?? string.Empty,

                                        Description =
                                            item.RestaurantDescription,

                                        Logo =
                                            item.RestaurantLogo,

                                        CoverImage =
                                            item.RestaurantCoverImage
                                    }
                            }
                        })
                        .ToList()
                    ?? new List<OrderItem>()
            };
        }
    }
}