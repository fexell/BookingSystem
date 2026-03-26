using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingSystem.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOldBookingsColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Surname",
                table: "Bookings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Bookings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Surname",
                table: "Bookings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
