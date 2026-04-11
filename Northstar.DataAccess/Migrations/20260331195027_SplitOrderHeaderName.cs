using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Northstar.Web.Migrations
{
    /// <inheritdoc />
    public partial class SplitOrderHeaderName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "OrderHeaders",
                newName: "LastName");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "OrderHeaders");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "OrderHeaders",
                newName: "Name");
        }
    }
}
