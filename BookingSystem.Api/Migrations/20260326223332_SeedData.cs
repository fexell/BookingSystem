using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookingSystem.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { 1, "role-admin-stamp-001", "Admin", "ADMIN" },
                    { 2, "role-user-stamp-002", "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "Surname", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { 1, 0, "admin-concurrency-stamp-001", "admin@studio.se", true, "Peter", false, null, "ADMIN@STUDIO.SE", "ADMIN", "AQAAAAIAAYagAAAAEFwcWzGVTcou6m4l3QfSB1EXixnj2xMRgwNCPUyCPdXWB7FxPFlwxCeszH1WVlxU+A==", null, false, "admin-security-stamp-001", "Andersson", false, "admin" },
                    { 2, 0, "anna-concurrency-stamp-002", "anna@user.com", true, "Anna", false, null, "ANNA@USER.COM", "ANNA", "AQAAAAIAAYagAAAAEI/PR+O8rGS5fyDzwjlV8EljBATSnF2r3ewX9LhFkrMR4FBn1zAWU+1Yvz+uPsWZlg==", null, false, "anna-security-stamp-002", "Svensson", false, "anna" },
                    { 3, 0, "erik-concurrency-stamp-003", "erik@example.com", true, "Erik", false, null, "ERIK@EXAMPLE.COM", "ERIK", "AQAAAAIAAYagAAAAENKjH98XL+b364+OivWCDlVt9xdw/rUR4JIT9geVokYZWpybdfuKp71pqEIJyPUOrg==", null, false, "erik-security-stamp-003", "Johansson", false, "erik" }
                });

            migrationBuilder.InsertData(
                table: "Resources",
                columns: new[] { "Id", "Description", "IsAvailable", "Name", "Type" },
                values: new object[,]
                {
                    { 1, "Big recording studio with drums, guitar amps, and a vocal booth.", true, "Studio A", "Studio" },
                    { 2, "Smaller studio, perfect for vocals and acoustic instruments.", true, "Studio B", "Studio" },
                    { 3, "Fender Twin Reverb, great for clean tones and classic rock.", true, "Guitar Amp", "Equipment" },
                    { 4, "Soundproof booth with a condenser microphone and pop filter.", true, "Vocal Booth", "Equipment" },
                    { 5, "Room with mixing console, monitors, and recording software.", true, "Control Room", "Studio" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 2 },
                    { 2, 3 }
                });

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "Id", "CreatedAt", "EndTime", "Notes", "PartySize", "ResourceId", "StartTime", "Status", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 3, 26, 10, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 27, 12, 0, 0, 0, DateTimeKind.Unspecified), "Need drumset", 2, 1, new DateTime(2026, 3, 27, 10, 0, 0, 0, DateTimeKind.Unspecified), "Active", 2 },
                    { 2, new DateTime(2026, 3, 26, 11, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 27, 15, 0, 0, 0, DateTimeKind.Unspecified), "Acoustic session", 1, 2, new DateTime(2026, 3, 27, 13, 0, 0, 0, DateTimeKind.Unspecified), "Active", 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 2, 3 });

            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
