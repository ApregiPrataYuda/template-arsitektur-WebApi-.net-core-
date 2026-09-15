using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace appOne.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuNavigationToSubMenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_ms_submenu_id_menu",
                table: "ms_submenu",
                column: "id_menu");

            migrationBuilder.AddForeignKey(
                name: "fk_ms_submenu_ms_menu_id_menu",
                table: "ms_submenu",
                column: "id_menu",
                principalTable: "ms_menu",
                principalColumn: "id_menu",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_ms_submenu_ms_menu_id_menu",
                table: "ms_submenu");

            migrationBuilder.DropIndex(
                name: "ix_ms_submenu_id_menu",
                table: "ms_submenu");
        }
    }
}
