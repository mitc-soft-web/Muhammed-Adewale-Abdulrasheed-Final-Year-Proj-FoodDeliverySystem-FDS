using FoodDeliverySystem.DTOs.Payment;
using FoodDeliverySystem.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace FoodDeliverySystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // =====================================================
        // LIST ALL PAYMENTS
        // =====================================================

        public async Task<IActionResult> Index(
            string? search,
            string? status,
            string? method,
            int page = 1)
        {
            const int pageSize = 10;

            var filter = new PaymentFilterDto
            {
                Search = search,
                Status = status,
                Method = method,
                Page = page,
                PageSize = pageSize
            };

            var result =
                await _paymentService.GetPaymentsAsync(filter);

            var statistics =
                await _paymentService.GetStatisticsAsync();

            ViewBag.CurrentPage = page;

            ViewBag.TotalPages =
                (int)Math.Ceiling(
                    result.TotalItems /
                    (double)pageSize);

            ViewBag.TotalItems =
                result.TotalItems;

            ViewBag.Search =
                search;

            ViewBag.Status =
                status;

            ViewBag.Method =
                method;

            ViewBag.TotalRevenue =
                statistics.TotalRevenue;

            ViewBag.TotalPaid =
                statistics.TotalPaid;

            ViewBag.TotalPending =
                statistics.TotalPending;

            ViewBag.TotalFailed =
                statistics.TotalFailed;

            // IMPORTANT:
            // Existing view continues receiving List<Payment>.
            return View(result.Payments);
        }


        // =====================================================
        // PAYMENT DETAILS
        // =====================================================

        public async Task<IActionResult> Details(int id)
        {
            var payment =
                await _paymentService
                    .GetPaymentDetailsAsync(id);

            if (payment == null)
            {
                return NotFound();
            }

            // Existing Details view continues receiving
            // the Payment entity exactly as before.
            return View(payment);
        }


        // =====================================================
        // EXPORT CSV
        // =====================================================

        public async Task<IActionResult> ExportCsv()
        {
            var payments =
                await _paymentService
                    .GetAllPaymentsForExportAsync();

            var builder =
                new StringBuilder();

            builder.AppendLine(
                "Payment ID,Order ID,Customer,Amount,Payment Method,Payment Status");


            foreach (var payment in payments)
            {
                builder.AppendLine(
                    $"{payment.Id}," +
                    $"{payment.OrderId}," +
                    $"\"{payment.Order?.Customer?.FullName}\"," +
                    $"{payment.Amount}," +
                    $"\"{payment.PaymentMethod}\"," +
                    $"\"{payment.PaymentStatus}\"");
            }


            return File(
                Encoding.UTF8.GetBytes(
                    builder.ToString()),
                "text/csv",
                $"Payments_{DateTime.Now:yyyyMMddHHmmss}.csv");
        }
    }
}