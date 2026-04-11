using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Demo.Main.Migrations
{
    /// <inheritdoc />
    public partial class AddedImageSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ProductImages",
                columns: new[] { "Id", "DisplayOrder", "ProductId", "Url" },
                values: new object[,]
                {
                    { 1, 0, 1, "\\images\\products\\arc_monitor_stand\\arc_monitor_stand_01.png" },
                    { 2, 1, 1, "\\images\\products\\arc_monitor_stand\\arc_monitor_stand_02.png" },
                    { 3, 2, 1, "\\images\\products\\arc_monitor_stand\\arc_monitor_stand_03.png" },
                    { 4, 3, 1, "\\images\\products\\arc_monitor_stand\\arc_monitor_stand_04.png" },
                    { 5, 0, 2, "\\images\\products\\ridge_desk_lamp\\ridge_desk_lamp_01.png" },
                    { 6, 1, 2, "\\images\\products\\ridge_desk_lamp\\ridge_desk_lamp_02.png" },
                    { 7, 2, 2, "\\images\\products\\ridge_desk_lamp\\ridge_desk_lamp_03.png" },
                    { 8, 3, 2, "\\images\\products\\ridge_desk_lamp\\ridge_desk_lamp_04.png" },
                    { 9, 0, 3, "\\images\\products\\atlas_desk_mat\\atlas_desk_mat_01.png" },
                    { 10, 1, 3, "\\images\\products\\atlas_desk_mat\\atlas_desk_mat_02.png" },
                    { 11, 0, 4, "\\images\\products\\pivot_laptop_riser\\pivot_laptop_riser_01.png" },
                    { 12, 1, 4, "\\images\\products\\pivot_laptop_riser\\pivot_laptop_riser_02.png" },
                    { 13, 0, 5, "\\images\\products\\harbor_wireless_keyboard\\harbor_wireless_keyboard_01.png" },
                    { 14, 1, 5, "\\images\\products\\harbor_wireless_keyboard\\harbor_wireless_keyboard_02.png" },
                    { 15, 2, 5, "\\images\\products\\harbor_wireless_keyboard\\harbor_wireless_keyboard_03.png" },
                    { 16, 3, 5, "\\images\\products\\harbor_wireless_keyboard\\harbor_wireless_keyboard_04.png" },
                    { 17, 0, 6, "\\images\\products\\harbor_wireless_mouse\\harbor_wireless_mouse_01.png" },
                    { 18, 1, 6, "\\images\\products\\harbor_wireless_mouse\\harbor_wireless_mouse_02.png" },
                    { 19, 0, 7, "\\images\\products\\signal_usb_c_hub\\signal_usb_c_hub_01.png" },
                    { 20, 1, 7, "\\images\\products\\signal_usb_c_hub\\signal_usb_c_hub_02.png" },
                    { 21, 0, 8, "\\images\\products\\dockline_charging_station\\dockline_charging_station_01.png" },
                    { 22, 1, 8, "\\images\\products\\dockline_charging_station\\dockline_charging_station_02.png" },
                    { 23, 0, 9, "\\images\\products\\fold_storage_box_set\\fold_storage_box_set_01.png" },
                    { 24, 1, 9, "\\images\\products\\fold_storage_box_set\\fold_storage_box_set_02.png" },
                    { 25, 0, 10, "\\images\\products\\grid_drawer_organizer\\grid_drawer_organizer_01.png" },
                    { 26, 1, 10, "\\images\\products\\grid_drawer_organizer\\grid_drawer_organizer_02.png" },
                    { 27, 2, 10, "\\images\\products\\grid_drawer_organizer\\grid_drawer_organizer_03.png" },
                    { 28, 3, 10, "\\images\\products\\grid_drawer_organizer\\grid_drawer_organizer_04.png" },
                    { 29, 0, 11, "\\images\\products\\stack_document_tray\\stack_document_tray_01.png" },
                    { 30, 1, 11, "\\images\\products\\stack_document_tray\\stack_document_tray_02.png" },
                    { 31, 0, 12, "\\images\\products\\rail_wall_hook_rack\\rail_wall_hook_rack_01.png" },
                    { 32, 1, 12, "\\images\\products\\rail_wall_hook_rack\\rail_wall_hook_rack_02.png" },
                    { 33, 0, 13, "\\images\\products\\transit_carry_backpack\\transit_carry_backpack_01.png" },
                    { 34, 1, 13, "\\images\\products\\transit_carry_backpack\\transit_carry_backpack_02.png" },
                    { 35, 2, 13, "\\images\\products\\transit_carry_backpack\\transit_carry_backpack_03.png" },
                    { 36, 3, 13, "\\images\\products\\transit_carry_backpack\\transit_carry_backpack_04.png" },
                    { 37, 0, 14, "\\images\\products\\harbor_laptop_sleeve\\harbor_laptop_sleeve_01.png" },
                    { 38, 1, 14, "\\images\\products\\harbor_laptop_sleeve\\harbor_laptop_sleeve_02.png" },
                    { 39, 0, 15, "\\images\\products\\roam_travel_pouch\\roam_travel_pouch_01.png" },
                    { 40, 1, 15, "\\images\\products\\roam_travel_pouch\\roam_travel_pouch_02.png" },
                    { 41, 0, 16, "\\images\\products\\summit_insulated_bottle\\summit_insulated_bottle_01.png" },
                    { 42, 1, 16, "\\images\\products\\summit_insulated_bottle\\summit_insulated_bottle_02.png" },
                    { 43, 0, 17, "\\images\\products\\vale_ambient_lamp\\vale_ambient_lamp_01.png" },
                    { 44, 1, 17, "\\images\\products\\vale_ambient_lamp\\vale_ambient_lamp_02.png" },
                    { 45, 2, 17, "\\images\\products\\vale_ambient_lamp\\vale_ambient_lamp_03.png" },
                    { 46, 3, 17, "\\images\\products\\vale_ambient_lamp\\vale_ambient_lamp_04.png" },
                    { 47, 0, 18, "\\images\\products\\hearth_utility_tray\\hearth_utility_tray_01.png" },
                    { 48, 1, 18, "\\images\\products\\hearth_utility_tray\\hearth_utility_tray_02.png" },
                    { 49, 0, 19, "\\images\\products\\drift_throw_blanket\\drift_throw_blanket_01.png" },
                    { 50, 1, 19, "\\images\\products\\drift_throw_blanket\\drift_throw_blanket_02.png" },
                    { 51, 0, 20, "\\images\\products\\grove_reed_diffuser\\grove_reed_diffuser_01.png" },
                    { 52, 1, 20, "\\images\\products\\grove_reed_diffuser\\grove_reed_diffuser_02.png" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 52);
        }
    }
}
