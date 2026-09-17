using FoodDeliverySystem.Services;
using FoodDeliverySystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FoodDeliverySystem.Models;

namespace FoodDeliverySystem.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICartService _cartService;
        private readonly ICheckoutService _checkoutService;

        public CartController(
            UserManager<ApplicationUser> userManager,
            ICartService cartService,
            ICheckoutService checkoutService)
        {
            _userManager = userManager;
            _cartService = cartService;
            _checkoutService = checkoutService;
        }

        // =========================================================
        // CUSTOMER CART
        // =========================================================

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            var cart = await _cartService
                .GetCartAsync(user.Id);

            if (cart == null)
            {
                return View(null);
            }

            var model = new Cart
            {
                Id = cart.Id,
                UserId = cart.UserId,

                CartItems = cart.CartItems
                    .Select(item => new CartItem
                    {
                        Id = item.Id,
                        CartId = item.CartId,
                        FoodItemId = item.FoodItemId,
                        Quantity = item.Quantity,

                        FoodItem = new FoodItem
                        {
                            Id = item.FoodItemId,
                            Name = item.FoodItemName,
                            Description =
                                item.FoodItemDescription,
                            ImageUrl =
                                item.FoodItemImageUrl,
                            Price =
                                item.FoodItemPrice
                        }
                    })
                    .ToList()
            };

            return View(model);
        }

        // =========================================================
        // CHECKOUT
        // =========================================================

        [HttpGet]
        public IActionResult Checkout()
        {
            var model = new CheckoutViewModel();

            return View(model);
        }

        public IActionResult OrderSuccess()
        {
            return View();
        }

        // =========================================================
        // PAYSTACK CALLBACK
        // =========================================================

        public async Task<IActionResult> PaymentCallback(
            string reference)
        {
            if (string.IsNullOrEmpty(reference))
            {
                TempData["Error"] =
                    "Payment reference was not received.";

                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            var orderId =
                HttpContext.Session.GetInt32("OrderId");

            var paymentId =
                HttpContext.Session.GetInt32("PaymentId");

            if (orderId == null || paymentId == null)
            {
                TempData["Error"] =
                    "Unable to locate your order.";

                return RedirectToAction(nameof(Index));
            }

            var result =
                await _checkoutService
                    .ProcessPaymentCallbackAsync(
                        reference,
                        orderId.Value,
                        paymentId.Value,
                        user.Id);

            if (!result.Success)
            {
                TempData["Error"] =
                    result.ErrorMessage
                    ?? "Payment verification failed.";

                return RedirectToAction(nameof(Index));
            }

            HttpContext.Session.Remove("OrderId");
            HttpContext.Session.Remove("PaymentId");

            TempData["Success"] =
                "Payment successful. Your order has been confirmed.";

            return RedirectToAction(nameof(OrderSuccess));
        }

        // =========================================================
        // CHECKOUT POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(
            CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            var callbackUrl = Url.Action(
                "PaymentCallback",
                "Cart",
                null,
                Request.Scheme);

            if (string.IsNullOrEmpty(callbackUrl))
            {
                TempData["Error"] =
                    "Unable to create payment callback URL.";

                return View(model);
            }

            var result =
                await _checkoutService.ProcessCheckoutAsync(
                    user.Id,
                    model,
                    user.Email ?? string.Empty,
                    callbackUrl);

            if (!result.Success)
            {
                TempData["Error"] =
                    result.ErrorMessage
                    ?? "Unable to complete your order.";

                return View(model);
            }

            if (result.OrderId.HasValue)
            {
                HttpContext.Session.SetInt32(
                    "OrderId",
                    result.OrderId.Value);
            }

            if (result.PaymentId.HasValue)
            {
                HttpContext.Session.SetInt32(
                    "PaymentId",
                    result.PaymentId.Value);
            }

            if (result.RedirectToPayment)
            {
                if (string.IsNullOrEmpty(result.PaymentUrl))
                {
                    TempData["Error"] =
                        "Unable to connect to Paystack.";

                    return View(model);
                }

                return Redirect(result.PaymentUrl);
            }

            TempData["Success"] =
                "Your order has been placed successfully.";

            return RedirectToAction(nameof(OrderSuccess));
        }

        // =========================================================
        // CART ACTIONS
        // =========================================================

        public async Task<IActionResult> AddToCart(int foodId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            try
            {
                await _cartService.AddToCartAsync(
                    user.Id,
                    foodId,
                    1);

                TempData["Success"] =
                    "Food added to cart.";

                return Redirect(
                    Request.Headers["Referer"].ToString());
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;

                return RedirectToAction(
                    "Index",
                    "FoodItem");
            }
        }

        public async Task<IActionResult> IncreaseQuantity(
            int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            await _cartService.IncreaseQuantityAsync(
                user.Id,
                id);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DecreaseQuantity(
            int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            await _cartService.DecreaseQuantityAsync(
                user.Id,
                id);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> RemoveItem(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            await _cartService.RemoveFromCartAsync(
                user.Id,
                id);

            TempData["Success"] =
                "Item removed from cart.";

            return RedirectToAction(nameof(Index));
        }
    }
}