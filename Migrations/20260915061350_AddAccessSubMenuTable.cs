using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace appOne.Migrations
{
    /// <inheritdoc />
    public partial class AddAccessSubMenuTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ms_access_submenu",
                columns: table => new
                {
                    id_access_sub_menu = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_user = table.Column<int>(type: "integer", nullable: false),
                    id_sub_menu = table.Column<int>(type: "integer", nullable: false),
                    can_view = table.Column<bool>(type: "boolean", nullable: false),
                    can_create = table.Column<bool>(type: "boolean", nullable: false),
                    can_update = table.Column<bool>(type: "boolean", nullable: false),
                    can_delete = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ms_access_submenu", x => x.id_access_sub_menu);
                    table.ForeignKey(
                        name: "fk_ms_access_submenu_ms_submenu_id_sub_menu",
                        column: x => x.id_sub_menu,
                        principalTable: "ms_submenu",
                        principalColumn: "id_submenu",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ms_access_submenu_ms_users_id_user",
                        column: x => x.id_user,
                        principalTable: "ms_users",
                        principalColumn: "id_user",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_ms_access_submenu_id_sub_menu",
                table: "ms_access_submenu",
                column: "id_sub_menu");

            migrationBuilder.CreateIndex(
                name: "ix_ms_access_submenu_id_user",
                table: "ms_access_submenu",
                column: "id_user");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ms_access_submenu");
        }
    }
}
