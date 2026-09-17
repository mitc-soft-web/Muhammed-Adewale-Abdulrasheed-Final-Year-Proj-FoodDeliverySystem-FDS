using FoodDeliverySystem.DTOs.Account;
using FoodDeliverySystem.Models;
using FoodDeliverySystem.Services;
using FoodDeliverySystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliverySystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(
        IAccountService accountService)
        {
            _accountService = accountService;
        }


        // REGISTER


        // Show Register Page
        public IActionResult Register()
        {
            return View();
        }

        // Register Customer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
        RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = new RegisterDto
            {
                FullName = model.FullName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Password = model.Password
            };

            var result =
                await _accountService.RegisterAsync(
                    dto,
                    Request.Scheme,
                    Request.Host.ToString());

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(
                        "Email",
                        error);
                }

                return View(model);
            }

            TempData["Success"] =
                "Registration successful! Please check your email to verify your account.";

            return RedirectToAction(nameof(Login));
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(
        string userId,
        string token)
        {
            var result =
                await _accountService.ConfirmEmailAsync(
                    userId,
                    token);

            if (!result.Succeeded)
            {
                TempData["Error"] =
                    result.Errors.FirstOrDefault()
                    ?? "Email verification failed. Please try again.";

                return RedirectToAction("Login");
            }

            TempData["Success"] =
                "Your email has been verified successfully. You can now log in.";

            return RedirectToAction("Login");
        }

        // LOGIN
        // Show Login Page
        public IActionResult Login()
        {
            return View();
        }

        // Login User
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(
          LoginViewModel model,
          string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = new LoginDto
            {
                Email = model.Email,
                Password = model.Password,
                RememberMe = model.RememberMe
            };

            var result =
                await _accountService.LoginAsync(dto);

            if (result.EmailNotConfirmed)
            {
                TempData["Error"] =
                    result.Errors.FirstOrDefault()
                    ?? "Please verify your email address before logging in.";

                return View(model);
            }

            if (result.RiderPending)
            {
                TempData["Error"] =
                    result.Errors.FirstOrDefault()
                    ?? "Your rider application is still under review.";

                return View(model);
            }

            if (result.RiderRejected)
            {
                TempData["Error"] =
                    result.Errors.FirstOrDefault()
                    ?? "Your rider application has been rejected.";

                return View(model);
            }

            if (!result.Succeeded)
            {
                ModelState.AddModelError(
                    string.Empty,
                    result.Errors.FirstOrDefault()
                    ?? "Invalid email or password.");

                return View(model);
            }

            TempData["Success"] = "Login successful. Welcome back!";

            if (!string.IsNullOrEmpty(returnUrl) &&
                Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            if (result.IsRider)
            {
                return RedirectToAction(
                    "Dashboard",
                    "Rider");
            }

            if (result.IsCustomer)
            {
                return RedirectToAction(
                    "Index",
                    "Home");
            }

            if (result.IsRestaurantOwner)
            {
                return RedirectToAction(
                    "Dashboard",
                    "Admin");
            }

            return RedirectToAction(
                "Index",
                "Home");
        }


        // LOGOUT
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();

            TempData["Success"] = "You have been logged out successfully.";

            return RedirectToAction(
                "Index",
                "Home");
        }


        // FORGOT PASSWORD
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(
        ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = new ForgotPasswordDto
            {
                Email = model.Email
            };

            var result =
                await _accountService.ForgotPasswordAsync(
                    dto,
                    Request.Scheme,
                    Request.Host.ToString());

            if (!result.Succeeded)
            {
                TempData["Error"] =
                    result.Errors.FirstOrDefault()
                    ?? "We couldn't send the password reset email right now. Please check your internet connection and try again.";

                return RedirectToAction(
                    nameof(ForgotPassword));
            }

            TempData["Success"] =
                "If the email exists in our system, a password reset link has been sent.";

            return RedirectToAction(
                nameof(ForgotPassword));
        }


        // RESET PASSWORD (GET)
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                return RedirectToAction("Login");
            }

            var model = new ResetPasswordViewModel
            {
                Token = token,
                Email = email
            };

            return View(model);
        }


        // RESET PASSWORD (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(
        ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = new ResetPasswordDto
            {
                Email = model.Email,
                Token = model.Token,
                Password = model.Password,
                ConfirmPassword = model.ConfirmPassword
            };

            var result =
                await _accountService.ResetPasswordAsync(dto);

            if (result.Succeeded)
            {
                TempData["Success"] =
                    "Your password has been reset successfully. You can now login.";

                return RedirectToAction(nameof(Login));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    "",
                    error);
            }

            return View(model);
        }


        [HttpGet]
        public IActionResult ResendVerification()
        {
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ResendVerification(string email)
        //{
        //    if (string.IsNullOrWhiteSpace(email))
        //    {
        //        TempData["Error"] = "Please enter your email address.";
        //        return View();
        //    }

        //    var user = await _userManager.FindByEmailAsync(email);

        //    if (user == null)
        //    {
        //        TempData["Error"] = "No account was found with that email.";
        //        return View();
        //    }

        //    if (await _userManager.IsEmailConfirmedAsync(user))
        //    {
        //        TempData["Success"] = "Your email is already verified. You can login.";
        //        return View();
        //    }

        //    try
        //    {
        //        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        //        var confirmationLink = Url.Action(
        //            "ConfirmEmail",
        //            "Account",
        //            new
        //            {
        //                userId = user.Id,
        //                token = token
        //            },
        //            Request.Scheme);

        //        var logoUrl = Url.Content("~/images/logo-light.png");
        //        var body = $@"

        //         <div style='text-align:center;margin-bottom:20px;'>

        //            <img src='{logoUrl}'
        //                 style='width:180px;'>

        //        </div>

        //        <h2>Food Xpress Email Verification</h2>

        //        <p>Hello <strong>{user.FullName}</strong>,</p>

        //        var logoUrl = $""{{Request.Scheme}}://{{Request.Host}}/images/logo-light.png"";

        //        <p>Please verify your email by clicking the button below.</p>

        //        <p>
        //        <a href='{confirmationLink}'
        //        style='background:#ff6b35;
        //        color:white;
        //        padding:12px 24px;
        //        text-decoration:none;
        //        border-radius:6px;'>

        //        Verify Email

        //        </a>
        //        </p>

        //        <hr>

        //        <small>Food Xpress Security Team</small>";

        //        await _emailService.SendEmailAsync(
        //            user.Email!,
        //            "Verify your Food Xpress account",
        //            body);

        //        TempData["Success"] =
        //            "A new verification email has been sent.";
        //    }
        //    catch
        //    {
        //        TempData["Error"] =
        //            "Unable to send verification email. Please check your internet connection and try again.";
        //    }

        //    return View();
        //}
    }
}