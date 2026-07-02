using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartPortfolioMonitor.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddInitialPortfolio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Name" },
                values: new object[] { 1, "shoh@example.com", "Shohjahon" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
