using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Traveling.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationUserMigration3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "90184155-dee0-40c9-bb1e-b5ed07afc04e",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2163148a-e0b0-47d6-988f-172d8cc684cb", "AQAAAAIAAYagAAAAEOsJwZCWDI2UR+k5rdvXBY33oFVDBDHKs46B/CURmVZ1pbxFbRrv/AYdX1XywbXCOw==", "fb27ea7b-ab98-4b97-9b53-f492b46deeac" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "90184155-dee0-40c9-bb1e-b5ed07afc04e",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2844b201-7b17-42b5-a268-758e8aaeae88", "AQAAAAIAAYagAAAAEGMJYT/86C0m0tMhULbcs/d7UjE1uMMzzA8Cq2Fqmd7LG6bpTBi34wHwUUWj7NWZfg==", "7190ee5d-ffee-45a4-84a7-ae4c39e94cc4" });
        }
    }
}
