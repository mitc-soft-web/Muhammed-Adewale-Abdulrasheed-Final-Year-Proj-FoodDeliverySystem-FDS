using FoodDeliverySystem.Models;
using FoodDeliverySystem.Services;
using FoodDeliverySystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliverySystem.Controllers
{
    public class RiderController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IDeliveryRiderService _deliveryRiderService;

        public RiderController(
            UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            IDeliveryRiderService deliveryRiderService)
        {
            _userManager = userManager;
            _emailService = emailService;
            _deliveryRiderService = deliveryRiderService;
        }

        // Admin - View All Rider Applications
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Index(
            string? search,
            string? status,
            string? vehicle)
        {
            var riders = await _deliveryRiderService.GetAllAsync(
                search,
                status,
                vehicle);

            ViewBag.Search = search;
            ViewBag.Status = status;
            ViewBag.Vehicle = vehicle;

            return View(riders);
        }

        // ADMIN - APPROVE RIDER
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            await _deliveryRiderService.ApproveAsync(id);

            TempData["Success"] =
                "Rider approved successfully.";

            return RedirectToAction(nameof(Index));
        }

        // ADMIN - VIEW RIDER DETAILS
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int id)
        {
            var rider =
                await _deliveryRiderService.GetByIdAsync(id);

            if (rider == null)
            {
                return NotFound();
            }

            return View(rider);
        }

        // ADMIN - REJECT RIDER
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(int id)
        {
            await _deliveryRiderService.RejectAsync(id);

            TempData["Success"] =
                "Rider rejected successfully.";

            return RedirectToAction(nameof(Index));
        }

        // RIDER DASHBOARD
        [Authorize(Roles = "Rider")]
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            var result =
                await _deliveryRiderService.GetDashboardAsync(user.Id);

            if (result == null)
            {
                return NotFound();
            }

            var dashboard = new RiderDashboardViewModel
            {
                Rider = result.Rider,

                NewOrders =
                    result.NewOrdersCount,

                ActiveOrders =
                    result.AssignedOrdersCount,

                CompletedDeliveries =
                    result.CompletedDeliveriesCount,

                TotalDeliveries =
                    result.TotalDeliveries,

                CashOrders =
                    result.CashOrders,

                OnlineOrders =
                    result.OnlineOrders,

                CashCollected =
                    result.CashCollected,

                OnlineCollected =
                    result.OnlineCollected,

                TotalEarnings =
                    result.TotalEarnings,

                TodayDeliveries =
                    result.TodayDeliveries,

                TodayEarnings =
                    result.TodayEarnings,

                ActiveDeliveries =
                    result.ActiveOrders,

                RecentDeliveries =
                    result.RecentDeliveries
            };

            return View(dashboard);
        }


        // TOGGLE RIDER AVAILABILITY
        [Authorize(Roles = "Rider")]
        public async Task<IActionResult> ToggleAvailability()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            await _deliveryRiderService.ToggleAvailabilityAsync(user.Id);

            return RedirectToAction(nameof(Dashboard));
        }

        // Show Rider Registration Page
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Handle Rider Registration
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RiderRegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if email already exists
            var existingUser = await _userManager.FindByEmailAsync(model.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "This email address is already registered.");

                return View(model);
            }

            string? licenseFileName = null;

            if (model.DriverLicenseFile != null)
            {
                string uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads",
                    "licenses");

                Directory.CreateDirectory(uploadsFolder);

                licenseFileName =
                    Guid.NewGuid().ToString() +
                    Path.GetExtension(model.DriverLicenseFile.FileName);

                string filePath = Path.Combine(uploadsFolder, licenseFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.DriverLicenseFile.CopyToAsync(stream);
                }
            }

            // Create Identity User
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }

            // Assign Rider Role
            await _userManager.AddToRoleAsync(user, "Rider");


            // Generate verification email

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var confirmationLink = Url.Action(
                "ConfirmEmail",
                "Account",
                new
                {
                    userId = user.Id,
                    token = token
                },
                Request.Scheme);


            var logoUrl = Url.Content("~/images/logo-light.png");
            var body = $@"
 <div style='text-align:center;margin-bottom:20px;'>

                    <img src='{logoUrl}'
                         style='width:180px;'>

                </div>

                <h2 style='text-align:center;color:#ff6b35;'>

                   Welcome to Food Xpress Rider Team

                </h2>

            <p>Hello <strong>{user.FullName}</strong>,</p>

            <p>

            Thank you for applying to become a Food Xpress Rider.

            </p>

            <p>

            Before we review your application,
            please verify your email address.

            </p>

            <p>

            <a href='{confirmationLink}'
            style='background:#ff6b35;
            color:white;
            padding:12px 24px;
            text-decoration:none;
            border-radius:6px;'>

            Verify Email

            </a>

            </p>

            <hr/>

            <small>

            Food Xpress Rider Team

            </small>";

            // Create Rider Profile
            var riderCreated =
        await _deliveryRiderService.RegisterRiderAsync(
            model,
            user.Id,
            licenseFileName);

            if (!riderCreated)
            {
                await _userManager.DeleteAsync(user);

                if (!string.IsNullOrEmpty(licenseFileName))
                {
                    var file = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "uploads",
                        "licenses",
                        licenseFileName);

                    if (System.IO.File.Exists(file))
                    {
                        System.IO.File.Delete(file);
                    }
                }

                TempData["Error"] =
                    "We couldn't submit your rider application. Please try again.";

                return View(model);
            }

            try
            {
                await _emailService.SendEmailAsync(
                    user.Email!,
                    "Verify your Food Xpress Rider account",
                    body);

                TempData["Success"] =
                    "Application submitted successfully. Please verify your email before your rider application can be reviewed.";

                return RedirectToAction(nameof(ApplicationSubmitted));
            }
            catch
            {
                var createdRider =
                    await _deliveryRiderService.GetByUserIdAsync(user.Id);

                if (createdRider != null)
                {
                    await _deliveryRiderService.DeleteAsync(
                        createdRider.Id);
                }

                await _userManager.DeleteAsync(user);

                if (!string.IsNullOrEmpty(licenseFileName))
                {
                    var file = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "uploads",
                        "licenses",
                        licenseFileName);

                    if (System.IO.File.Exists(file))
                    {
                        System.IO.File.Delete(file);
                    }
                }

                TempData["Error"] =
                    "We couldn't send your verification email. Please check your internet connection and try again.";

                return View(model);
            }
        }

        public IActionResult ApplicationSubmitted()
        {
            return View();
        }

        [Authorize(Roles = "Rider")]
        public async Task<IActionResult> AcceptOrder(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            var success = await _deliveryRiderService.AcceptOrderAsync(
                user.Id,
                id);

            if (!success)
            {
                return NotFound();
            }

            TempData["Success"] =
                "Delivery accepted successfully.";

            return RedirectToAction(nameof(Dashboard));
        }

        [Authorize(Roles = "Rider")]
        public async Task<IActionResult> AssignedOrders()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            var orders =
                await _deliveryRiderService.GetAssignedOrdersAsync(
                    user.Id);

            return View(orders);
        }


        //Delivery History

        [Authorize(Roles = "Rider")]
        public async Task<IActionResult> DeliveryHistory(
         string search,
         string paymentMethod,
         string quickFilter,
         DateTime? fromDate,
         DateTime? toDate)
        {
            var user =
                await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            var result =
                await _deliveryRiderService
                    .GetDeliveryHistorySummaryAsync(
                        user.Id,
                        search,
                        paymentMethod,
                        quickFilter,
                        fromDate,
                        toDate);

            ViewBag.TotalDeliveries =
                result.TotalDeliveries;

            ViewBag.CashOrders =
                result.CashOrders;

            ViewBag.OnlineOrders =
                result.OnlineOrders;

            ViewBag.TotalEarnings =
                result.TotalEarnings;

            // Preserve filter values
            ViewBag.Search = search;

            ViewBag.PaymentMethod =
                paymentMethod;

            ViewBag.QuickFilter =
                quickFilter;

            ViewBag.FromDate =
                fromDate;

            ViewBag.ToDate =
                toDate;

            return View(result.Deliveries);
        }

        [Authorize(Roles = "Rider")]
        public async Task<IActionResult> CompleteOrder(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            var success =
                await _deliveryRiderService.CompleteOrderAsync(
                    user.Id,
                    id);

            if (!success)
            {
                TempData["Error"] =
                    "You must accept this delivery before you can complete it.";

                return RedirectToAction(
                    nameof(OrderDetails),
                    new { id });
            }

            TempData["Success"] =
                "🎉 Delivery completed successfully.";

            return RedirectToAction(nameof(AssignedOrders));
        }


        // RIDER - ORDER DETAILS
        [Authorize(Roles = "Rider")]
        public async Task<IActionResult> OrderDetails(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            var order = await _deliveryRiderService
                .GetRiderOrderDetailsAsync(user.Id, id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }


        [Authorize(Roles = "Rider")]
        public async Task<IActionResult> Receipt(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            var order =
                await _deliveryRiderService.GetRiderReceiptAsync(
                    user.Id,
                    id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}