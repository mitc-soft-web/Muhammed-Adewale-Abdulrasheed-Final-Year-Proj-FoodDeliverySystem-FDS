using FoodDeliverySystem.DTOs.Account;

namespace FoodDeliverySystem.Services
{
    public interface IAccountService
    {
        Task<AccountServiceResult> RegisterAsync(
        RegisterDto dto,
        string requestScheme,
        string requestHost);

        Task<AccountServiceResult> ConfirmEmailAsync(
            string userId,
            string token);

        Task<AccountLoginResult> LoginAsync(
            LoginDto dto);

        Task LogoutAsync();

        Task<AccountServiceResult> ForgotPasswordAsync(
        ForgotPasswordDto dto,
        string requestScheme,
        string requestHost);

        Task<AccountServiceResult> ResetPasswordAsync(
            ResetPasswordDto dto);
    }

    public class AccountServiceResult
    {
        public bool Succeeded { get; set; }

        public List<string> Errors { get; set; } = new();

        public static AccountServiceResult Success()
        {
            return new AccountServiceResult
            {
                Succeeded = true
            };
        }

        public static AccountServiceResult Failure(
            params string[] errors)
        {
            return new AccountServiceResult
            {
                Succeeded = false,
                Errors = errors.ToList()
            };
        }
    }

    public class AccountLoginResult
    {
        public bool Succeeded { get; set; }

        public bool EmailNotConfirmed { get; set; }

        public bool RiderPending { get; set; }

        public bool RiderRejected { get; set; }

        public bool InvalidCredentials { get; set; }

        public bool IsRider { get; set; }

        public bool IsCustomer { get; set; }

        public bool IsRestaurantOwner { get; set; }

        public List<string> Errors { get; set; } = new();
    }
}