using FoodDeliverySystem.Configurations;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace FoodDeliverySystem.Services
{
    public class PaystackService
    {
        private readonly HttpClient _httpClient;
        private readonly PaystackSettings _settings;

        public PaystackService(
            HttpClient httpClient,
            IOptions<PaystackSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;

            _httpClient.DefaultRequestHeaders.Clear();

            _httpClient.DefaultRequestHeaders.Add(
                "Authorization",
                $"Bearer {_settings.SecretKey}");
        }

        public async Task<string?> InitializePayment(
            string email,
            decimal amount,
            string callbackUrl)
        {
            var request = new
            {
                email = email,
                amount = (int)(amount * 100),
                callback_url = callbackUrl
            };

            var json = JsonConvert.SerializeObject(request);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(
                "https://api.paystack.co/transaction/initialize",
                content);

            var responseString = await response.Content.ReadAsStringAsync();

            dynamic result = JsonConvert.DeserializeObject(responseString)!;

            if (result.status == true)
            {
                return result.data.authorization_url;
            }

            return null;
        }

        public async Task<dynamic?> VerifyPayment(string reference)
        {
            var response = await _httpClient.GetAsync(
                $"https://api.paystack.co/transaction/verify/{reference}");

            var responseString = await response.Content.ReadAsStringAsync();

            dynamic result = JsonConvert.DeserializeObject(responseString)!;

            if (result.status == true)
            {
                return result.data;
            }

            return null;
        }
    }
}