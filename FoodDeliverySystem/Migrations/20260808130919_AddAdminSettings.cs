using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodDeliverySystem.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Theme = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SidebarCollapsed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    RememberSidebarState = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    NotificationsEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    NewOrderNotifications = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    RiderApplicationNotifications = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    RiderStatusNotifications = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PaymentNotifications = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    RestaurantNotifications = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SystemNotifications = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Language = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Currency = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateFormat = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TimeFormat = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DefaultDashboard = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ShowRestaurantCard = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ShowCategoryCard = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ShowFoodItemCard = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ShowOrderCard = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ShowRiderCard = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ShowRevenueCard = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AutoRefreshDashboard = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ActivityTrackingEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    RememberLogin = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdminSettings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AdminSettings_UserId",
                table: "AdminSettings",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminSettings");
        }
    }
}
