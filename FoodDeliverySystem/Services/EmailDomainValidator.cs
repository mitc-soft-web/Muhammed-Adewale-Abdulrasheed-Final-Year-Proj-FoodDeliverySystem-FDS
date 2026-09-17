using System.Net;

namespace FoodDeliverySystem.Services
{
    public class EmailDomainValidator
    {
        public async Task<bool> DomainExists(string email)
        {
            try
            {
                var domain = email.Split('@').Last();

                await Dns.GetHostEntryAsync(domain);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}