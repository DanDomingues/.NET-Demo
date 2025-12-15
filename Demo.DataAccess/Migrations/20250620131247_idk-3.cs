using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP.NET_Debut.Migrations
{
    /// <inheritdoc />
    public partial class idk3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotallyNotAnID",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 2,
                column: "TotallyNotAnID",
                value: 3);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotallyNotAnID",
                table: "Product");
        }
    }
}
