using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyWebApi.Migrations
{
    /// <inheritdoc />
    public partial class addAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Address", "City", "CreatedAt", "Email", "Name", "Password", "PhoneNumber", "Role" },
                values: new object[] { 2, "abc", "aaa", new DateTime(2025, 8, 4, 11, 38, 22, 216, DateTimeKind.Utc).AddTicks(9579), "admin@gmail.com", "Admin", "admin123", "03313", "Admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            

            //migrationBuilder.CreateIndex(
            //    name: "IX_VoucherUse_VoucherId",
            //    table: "VoucherUse",
            //    column: "VoucherId");
        }
    }
}
