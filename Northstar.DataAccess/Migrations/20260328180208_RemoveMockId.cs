using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.Main.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMockId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotallyNotAnID",
                table: "Products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotallyNotAnID",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "TotallyNotAnID",
                value: 0);
        }
    }
}
