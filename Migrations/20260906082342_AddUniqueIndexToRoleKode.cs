using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace appOne.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexToRoleKode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_ms_role_kode_role",
                table: "ms_role",
                column: "kode_role",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_ms_role_kode_role",
                table: "ms_role");
        }
    }
}
