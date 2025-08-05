using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyWebApi.Migrations
{
    /// <inheritdoc />
    public partial class newAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2025, 8, 5, 10, 24, 35, 285, DateTimeKind.Utc).AddTicks(1296), "$2a$11$zBh2eXD57XRypRGCAZmvoOCuA/BHAJMK9IKImpASpxeCrJmkKcmwa" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2025, 8, 4, 11, 38, 22, 216, DateTimeKind.Utc).AddTicks(9579), "admin123" });
        }
    }
}
