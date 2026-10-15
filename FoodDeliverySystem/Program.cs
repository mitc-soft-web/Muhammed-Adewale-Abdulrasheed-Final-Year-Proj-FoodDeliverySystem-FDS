using FoodDeliverySystem.Configurations;
using FoodDeliverySystem.Data;
using FoodDeliverySystem.Interfaces;
using FoodDeliverySystem.Models;
using FoodDeliverySystem.Repositories;
using FoodDeliverySystem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliverySystem
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add<FoodDeliverySystem.Filters.AdminNotificationFilter>();
            });
            builder.Services.AddSession();
            builder.Services.AddSignalR();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<ICartRepository, CartRepository>();

            builder.Services.AddScoped<EmailDomainValidator>();
            builder.Services.AddScoped<ICheckoutService, CheckoutService>();

            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();

            builder.Services.AddScoped<IFoodItemRepository, FoodItemRepository>();
            builder.Services.AddScoped<IFoodItemService, FoodItemService>();

            builder.Services.AddScoped<IRestaurantRepository, RestaurantRepository>();
            builder.Services.AddScoped<IRestaurantService, RestaurantService>();

            builder.Services.AddScoped<IFoodCategoryRepository, FoodCategoryRepository>();
            builder.Services.AddScoped<IFoodCategoryService, FoodCategoryService>();

            builder.Services.AddScoped<IDeliveryRiderRepository, DeliveryRiderRepository>();
            builder.Services.AddScoped<IDeliveryRiderService, DeliveryRiderService>();

            builder.Services.AddScoped<IAccountService, AccountService>();

            builder.Services.AddScoped<IAdminRepository, AdminRepository>();
            builder.Services.AddScoped<IAdminService, AdminService>();

            builder.Services.AddScoped<ISearchRepository, SearchRepository>();
            builder.Services.AddScoped<ISearchService, SearchService>();

            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();

            builder.Services.AddScoped<IHomeRepository, HomeRepository>();
            builder.Services.AddScoped<IHomeService, HomeService>();

            builder.Services.AddScoped<ITrackingService, TrackingService>();

            // Configure MySQL DbContext
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(
                    builder.Configuration.GetConnectionString("FDSContext"),
                    new MySqlServerVersion(new Version(8, 4, 0))
                ));

            // Configure Identity
            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

            // Configure Authentication Cookie
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/Login";
            });

            builder.Services.Configure<PaystackSettings>(
                builder.Configuration.GetSection("Paystack"));

            builder.Services.AddHttpClient<PaystackService>();

            builder.Services.AddScoped<IEmailService, EmailService>();

            var app = builder.Build();

            // Apply database migrations before seeding roles and the admin user.
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var dbContext =
                    services.GetRequiredService<ApplicationDbContext>();

                await dbContext.Database.MigrateAsync();

                // Seed Roles
                var roleManager =
                    services.GetRequiredService<RoleManager<IdentityRole>>();

                string[] roles =
                {
                    "Admin",
                    "Customer",
                    "RestaurantOwner",
                    "Rider"
                };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        var result = await roleManager.CreateAsync(
                            new IdentityRole(role));

                        if (!result.Succeeded)
                        {
                            var errors = string.Join(
                                "; ",
                                result.Errors.Select(error => error.Description));

                            throw new InvalidOperationException(
                                $"Failed to create role '{role}': {errors}");
                        }
                    }
                }

                var userManager =
                    services.GetRequiredService<UserManager<ApplicationUser>>();

                await FoodDeliverySystem.SeedData.AdminSeeder
                    .SeedAdminAsync(userManager);
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            app.MapHub<FoodDeliverySystem.Hubs.TrackingHub>("/trackingHub");

            app.Run();
        }
    }
}