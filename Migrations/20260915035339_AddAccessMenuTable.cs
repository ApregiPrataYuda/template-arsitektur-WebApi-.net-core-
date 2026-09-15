using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace appOne.Migrations
{
    /// <inheritdoc />
    public partial class AddAccessMenuTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ms_access_menu",
                columns: table => new
                {
                    id_access_menu = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_role = table.Column<int>(type: "integer", nullable: false),
                    id_menu = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ms_access_menu", x => x.id_access_menu);
                    table.ForeignKey(
                        name: "fk_ms_access_menu_ms_menu_id_menu",
                        column: x => x.id_menu,
                        principalTable: "ms_menu",
                        principalColumn: "id_menu",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ms_access_menu_ms_role_id_role",
                        column: x => x.id_role,
                        principalTable: "ms_role",
                        principalColumn: "id_role",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_ms_access_menu_id_menu",
                table: "ms_access_menu",
                column: "id_menu");

            migrationBuilder.CreateIndex(
                name: "ix_ms_access_menu_id_role",
                table: "ms_access_menu",
                column: "id_role");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ms_access_menu");
        }
    }
}
