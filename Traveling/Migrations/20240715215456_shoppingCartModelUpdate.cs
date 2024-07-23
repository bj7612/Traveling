using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Traveling.Migrations
{
    /// <inheritdoc />
    public partial class shoppingCartModelUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "ShoppingCarts",
                newName: "Id");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "90184155-dee0-40c9-bb1e-b5ed07afc04e",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e8c951f8-a75a-4ee4-bcdc-f1a5c90e67db", "AQAAAAIAAYagAAAAENU2McryG/d6/ffAhrcL05swoIWCWEmB4uscZfho9KPjUu9ESK3b5T7ONQ+I/FqrUA==", "ba1ce2ed-e290-4876-85ee-63fa05a4856c" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ShoppingCarts",
                newName: "id");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "90184155-dee0-40c9-bb1e-b5ed07afc04e",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7319c916-0ca9-412a-bf63-546f9a0b3230", "AQAAAAIAAYagAAAAEK+IDQ8puhOZhdFQJhOXWyvSEEmElTuJr0MUSwfVg2vW2SVfDhuQWvddPTSJMOnpAA==", "1af984dd-d1c2-490d-b92f-37f9fb4914a4" });
        }
    }
}
