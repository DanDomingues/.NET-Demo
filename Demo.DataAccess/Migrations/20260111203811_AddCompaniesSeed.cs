using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ASP.NET_Debut.Migrations
{
    /// <inheritdoc />
    public partial class AddCompaniesSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Companies",
                columns: ["Id", "City", "Name", "PhoneNumber", "PostalCode", "State", "StreetAddress"],
                values: new object[,]
                {
                    { 1, null, "D Amaze-on", null, null, null, null },
                    { 2, null, "Ye old bookshop", null, null, null, null },
                    { 3, null, "Tito books", null, null, null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
