using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP.NET_Debut.Migrations
{
    /// <inheritdoc />
    public partial class idk2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.InsertData(
                table: "Product",
                columns: new[] { "Id", "Author", "Description", "ISBN", "Price", "Price100", "Price50", "Title" },
                values: new object[] { 2, "Billy Spark", "Lorem ipsum", "SWD999901", 90.0, 80.0, 85.0, "Fortune of Time" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.InsertData(
                table: "Product",
                columns: new[] { "Id", "Author", "Description", "ISBN", "Price", "Price100", "Price50", "Title" },
                values: new object[] { 1, "Billy Spark", "Lorem ipsum", "SWD999901", 90.0, 80.0, 85.0, "Fortune of Time" });
        }
    }
}
