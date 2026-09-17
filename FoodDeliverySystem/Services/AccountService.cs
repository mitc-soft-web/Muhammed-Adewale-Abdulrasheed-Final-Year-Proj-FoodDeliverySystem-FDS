using FoodDeliverySystem.Data;
using FoodDeliverySystem.DTOs.Account;
using FoodDeliverySystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliverySystem.Services
{
    public class AccountService : IAccountService
    {
        private readonly IDeliveryRiderService _deliveryRiderService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public AccountService(
            UserManager<ApplicationUser> userManager,
            IDeliveryRiderService deliveryRiderService,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _emailService = emailService;
            _deliveryRiderService = deliveryRiderService;
        }

        public async Task<AccountServiceResult> RegisterAsync(
     RegisterDto dto,
     string requestScheme,
     string requestHost)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var existingUser =
                    await _userManager.FindByEmailAsync(dto.Email);

                if (existingUser != null)
                {
                    await transaction.RollbackAsync();

                    return AccountServiceResult.Failure(
                        "This email address is already registered.");
                }

                var user = new ApplicationUser
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    FullName = dto.FullName,
                    PhoneNumber = dto.PhoneNumber
                };

                var result =
                    await _userManager.CreateAsync(
                        user,
                        dto.Password);

                if (!result.Succeeded)
                {
                    await transaction.RollbackAsync();

                    return AccountServiceResult.Failure(
                        result.Errors
                            .Select(e => e.Description)
                            .ToArray());
                }

                var roleResult =
                    await _userManager.AddToRoleAsync(
                        user,
                        "Customer");

                if (!roleResult.Succeeded)
                {
                    await transaction.RollbackAsync();

                    return AccountServiceResult.Failure(
                        roleResult.Errors
                            .Select(e => e.Description)
                            .ToArray());
                }

                var token =
                    await _userManager.GenerateEmailConfirmationTokenAsync(
                        user);

                var confirmationLink =
                    $"{requestScheme}://{requestHost}" +
                    $"/Account/ConfirmEmail" +
                    $"?userId={Uri.EscapeDataString(user.Id)}" +
                    $"&token={Uri.EscapeDataString(token)}";

                var body = $@"
                <div style='text-align:center;margin-bottom:20px;'>
                    <img src='images/logo-light.png'
                         style='width:180px;'>
                </div>

                <h2 style='text-align:center;color:#ff6b35;'>
                    Welcome to Food Xpress
                </h2>

                <p>Hello <strong>{user.FullName}</strong>,</p>

                <p>
                    Thank you for creating your Food Xpress account.
                </p>

                <p>
                    Please confirm your email address by clicking the button below.
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
                    Food Xpress Security Team
                </small>";

                await _emailService.SendEmailAsync(
                    user.Email!,
                    "Verify your Food Xpress account",
                    body);

                await transaction.CommitAsync();

                return AccountServiceResult.Success();
            }
            catch (InvalidOperationException ex)
            {
                await transaction.RollbackAsync();

                return AccountServiceResult.Failure(
                    ex.Message);
            }
            catch
            {
                await transaction.RollbackAsync();

                return AccountServiceResult.Failure(
                    "We couldn't complete your registration right now. Please check your internet connection and try again.");
            }
        }

        public async Task<AccountServiceResult> ConfirmEmailAsync(
        string userId,
        string token)
        {
            var user =
                await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return AccountServiceResult.Failure(
                    "Email verification failed. Please try again.");
            }

            var result =
                await _userManager.ConfirmEmailAsync(
                    user,
                    token);

            if (!result.Succeeded)
            {
                return AccountServiceResult.Failure(
                    "Email verification failed. Please try again.");
            }

            return AccountServiceResult.Success();
        }


        public async Task<AccountLoginResult> LoginAsync(
        LoginDto dto)
        {
            var user =
                await _userManager.FindByEmailAsync(dto.Email);

            if (user != null &&
                !await _userManager.IsEmailConfirmedAsync(user))
            {
                return new AccountLoginResult
                {
                    EmailNotConfirmed = true,
                    Errors =
                    {
                        "Please verify your email address before logging in. Check your Inbox or Spam folder."
                    }
                };
            }

            if (user != null &&
                await _userManager.IsInRoleAsync(
                    user,
                    "Rider"))
            {
                var rider =
                await _deliveryRiderService
                    .GetByUserIdAsync(user.Id);

                if (rider != null)
                {
                    if (rider.Status == "Pending")
                    {
                        return new AccountLoginResult
                        {
                            RiderPending = true,
                            Errors =
                            {
                                "Your rider application is still under review. Please wait for admin approval."
                            }
                        };
                    }

                    if (rider.Status == "Rejected")
                    {
                        return new AccountLoginResult
                        {
                            RiderRejected = true,
                            Errors =
                            {
                                "Your rider application has been rejected. Please contact support."
                            }
                        };
                    }
                }
            }

            var result =
                await _signInManager.PasswordSignInAsync(
                    dto.Email,
                    dto.Password,
                    dto.RememberMe,
                    lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return new AccountLoginResult
                {
                    InvalidCredentials = true,
                    Errors =
                    {
                        "Invalid email or password."
                    }
                };
            }

            return new AccountLoginResult
            {
                Succeeded = true,
                IsRider =
                    user != null &&
                    await _userManager.IsInRoleAsync(
                        user,
                        "Rider"),
                IsCustomer =
                    user != null &&
                    await _userManager.IsInRoleAsync(
                        user,
                        "Customer"),
                IsRestaurantOwner =
                    user != null &&
                    await _userManager.IsInRoleAsync(
                        user,
                        "RestaurantOwner")
            };
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<AccountServiceResult> ForgotPasswordAsync(
         ForgotPasswordDto dto,
         string requestScheme,
         string requestHost)
        {
            var user =
                await _userManager.FindByEmailAsync(dto.Email);

            if (user == null ||
                !await _userManager.IsEmailConfirmedAsync(user))
            {
                return AccountServiceResult.Success();
            }

            var token =
                await _userManager.GeneratePasswordResetTokenAsync(
                    user);

            var resetLink =
                $"{requestScheme}://{requestHost}" +
                $"/Account/ResetPassword" +
                $"?token={Uri.EscapeDataString(token)}" +
                $"&email={Uri.EscapeDataString(user.Email!)}";

            var body = $@"
            <div style='text-align:center;margin-bottom:20px;'>
                <img src='images/logo-light.png'
                     style='width:180px;'>
            </div>

            <p>Hello <strong>{user.FullName}</strong>,</p>

            <p>You requested to reset your password.</p>

            <p>
                Click the button below to reset your password.
            </p>

            <p>
                <a href='{resetLink}'
                   style='background:#ff6b35;
                          color:white;
                          padding:12px 24px;
                          text-decoration:none;
                          border-radius:6px;'>
                    Reset Password
                </a>
            </p>

            <p>
                If you didn't request this,
                simply ignore this email.
            </p>

            <hr>

            <small>
                Food Xpress Security Team
            </small>";

            try
            {
                await _emailService.SendEmailAsync(
                    user.Email!,
                    "Food Xpress Password Reset",
                    body);

                return AccountServiceResult.Success();
            }
            catch (InvalidOperationException)
            {
                return AccountServiceResult.Failure(
                    "We couldn't send the password reset email right now. Please check your internet connection and try again.");
            }
        }


        public async Task<AccountServiceResult> ResetPasswordAsync(
        ResetPasswordDto dto)
        {
            var user =
                await _userManager.FindByEmailAsync(
                    dto.Email);

            if (user == null)
            {
                return AccountServiceResult.Failure();
            }

            var result =
                await _userManager.ResetPasswordAsync(
                    user,
                    dto.Token,
                    dto.Password);

            if (!result.Succeeded)
            {
                return AccountServiceResult.Failure(
                    result.Errors
                        .Select(e => e.Description)
                        .ToArray());
            }

            return AccountServiceResult.Success();
        }
    }
}