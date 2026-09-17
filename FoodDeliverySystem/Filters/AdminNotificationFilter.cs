using FoodDeliverySystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliverySystem.Filters
{
    public class AdminNotificationFilter : IAsyncActionFilter
    {
        private readonly ApplicationDbContext _context;

        public AdminNotificationFilter(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var result = await next();

            if (result.Controller is Controller controller)
            {
                controller.ViewBag.UnreadCount =
                    await _context.Notifications
                    .CountAsync(x => !x.IsRead);
            }
        }
    }
}