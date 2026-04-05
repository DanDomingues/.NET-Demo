using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Demo.Main.Migrations
{
    /// <inheritdoc />
    public partial class UpdateContentSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categories",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(12)",
                oldMaxLength: 12);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Workspace");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Tech Accessories");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Organization");

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "DisplayOrder", "Name" },
                values: new object[,]
                {
                    { 4, 4, "Travel" },
                    { 5, 5, "Home Essentials" }
                });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Apex Office Solutions");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "HarborTech Wholesale");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "ModuLiving Distributors");

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "City", "Name", "PhoneNumber", "PostalCode", "State", "StreetAddress" },
                values: new object[,]
                {
                    { 4, null, "TransitPro Supply", null, null, null, null },
                    { 5, null, "Hearth & Home Trade", null, null, null, null }
                });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Author", "CategoryId", "Description", "ISBN", "Price", "Price100", "Price50", "Title" },
                values: new object[] { "Luma Works", 1, "Minimal LED desk lamp with an adjustable neck and warm ambient lighting for focused work sessions.", "NSS-WS-002", 79.0, 69.0, 0.0, "Ridge Desk Lamp" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Author", "CategoryId", "Description", "ISBN", "Name", "Price", "Price100", "Price50", "Title" },
                values: new object[,]
                {
                    { 1, "Northstar Studio", 1, "Elevated aluminum monitor stand designed to create a cleaner desk setup and improve everyday screen ergonomics.", "NSS-WS-001", null, 89.0, 79.0, 0.0, "Arc Monitor Stand" },
                    { 3, "Atlas Goods", 1, "Soft-touch desk mat that anchors keyboards, mice, and notebooks while adding a refined layer to the workspace.", "NSS-WS-003", null, 35.0, 29.0, 0.0, "Atlas Desk Mat" },
                    { 4, "Pivot Labs", 1, "Foldable laptop riser built for better posture, cleaner airflow, and flexible work-from-home setups.", "NSS-WS-004", null, 59.0, 52.0, 0.0, "Pivot Laptop Riser" },
                    { 5, "Harbor Works", 2, "Slim wireless keyboard with a clean layout and quiet typing feel, designed for modern desk setups.", "NSS-TA-001", null, 99.0, 89.0, 0.0, "Harbor Wireless Keyboard" },
                    { 6, "Harbor Works", 2, "Compact wireless mouse with an ergonomic silhouette and smooth tracking for daily productivity.", "NSS-TA-002", null, 49.0, 43.0, 0.0, "Harbor Wireless Mouse" },
                    { 7, "Signal Labs", 2, "Multi-port USB-C hub that expands laptop connectivity with a sleek footprint for travel or desk use.", "NSS-TA-003", null, 69.0, 61.0, 0.0, "Signal USB-C Hub" },
                    { 8, "Dockline Systems", 2, "Streamlined charging station for phones, earbuds, and accessories, built to reduce cable clutter.", "NSS-TA-004", null, 79.0, 69.0, 0.0, "Dockline Charging Station" },
                    { 9, "Fold Home", 3, "Set of collapsible storage boxes that helps keep shelves, closets, and work areas visually organized.", "NSS-OR-001", null, 39.0, 33.0, 0.0, "Fold Storage Box Set" },
                    { 10, "Grid House", 3, "Modular drawer organizer designed for office supplies, personal accessories, and small everyday items.", "NSS-OR-002", null, 29.0, 25.0, 0.0, "Grid Drawer Organizer" },
                    { 11, "Stack Office", 3, "Layered document tray with a clean silhouette for sorting paperwork, notebooks, and incoming mail.", "NSS-OR-003", null, 34.0, 28.0, 0.0, "Stack Document Tray" },
                    { 12, "Rail Utility", 3, "Simple wall-mounted hook rack for entryways, offices, and utility spaces that need practical storage.", "NSS-OR-004", null, 32.0, 26.0, 0.0, "Rail Wall Hook Rack" },
                    { 13, "Transit Co.", 4, "Daily carry backpack with a streamlined profile, laptop compartment, and versatile storage for commuting.", "NSS-TR-001", null, 129.0, 115.0, 0.0, "Transit Carry Backpack" },
                    { 14, "Harbor Works", 4, "Protective laptop sleeve with a minimalist exterior and soft inner lining for daily transport.", "NSS-TR-002", null, 45.0, 39.0, 0.0, "Harbor Laptop Sleeve" },
                    { 15, "Roam Supply", 4, "Compact organizer pouch for chargers, passports, toiletries, and other travel essentials.", "NSS-TR-003", null, 28.0, 24.0, 0.0, "Roam Travel Pouch" },
                    { 16, "Summit Goods", 4, "Insulated stainless steel bottle that keeps drinks at the right temperature through commutes and day trips.", "NSS-TR-004", null, 33.0, 27.0, 0.0, "Summit Insulated Bottle" },
                    { 17, "Vale Living", 5, "Soft ambient table lamp created to bring warmth and calm lighting into bedrooms, living rooms, and reading corners.", "NSS-HE-001", null, 74.0, 64.0, 0.0, "Vale Ambient Lamp" },
                    { 18, "Hearth Studio", 5, "Everyday catch-all tray for keys, wallets, candles, and small essentials that tend to gather around the home.", "NSS-HE-002", null, 26.0, 22.0, 0.0, "Hearth Utility Tray" },
                    { 19, "Drift Home", 5, "Textured throw blanket designed to add warmth, softness, and a lived-in feel to sofas and beds.", "NSS-HE-003", null, 49.0, 43.0, 0.0, "Drift Throw Blanket" },
                    { 20, "Grove Living", 5, "Subtle reed diffuser with a clean vessel and balanced fragrance profile for calm interior spaces.", "NSS-HE-004", null, 31.0, 26.0, 0.0, "Grove Reed Diffuser" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categories",
                type: "nvarchar(12)",
                maxLength: 12,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Action");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Fiction");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Drama");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "D Amaze-on");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Ye old bookshop");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Tito books");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Author", "CategoryId", "Description", "ISBN", "Price", "Price100", "Price50", "Title" },
                values: new object[] { "Billy Spark", 3, "Lorem ipsum", "SWD999901", 90.0, 80.0, 85.0, "Fortune of Time" });
        }
    }
}
