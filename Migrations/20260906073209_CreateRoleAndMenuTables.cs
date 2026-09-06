using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace appOne.Migrations
{
    /// <inheritdoc />
    public partial class CreateRoleAndMenuTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ms_menu",
                columns: table => new
                {
                    id_menu = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    kode_menu = table.Column<string>(type: "text", nullable: false),
                    menu = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ms_menu", x => x.id_menu);
                });

            migrationBuilder.CreateTable(
                name: "ms_role",
                columns: table => new
                {
                    id_role = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    kode_role = table.Column<string>(type: "text", nullable: false),
                    role = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ms_role", x => x.id_role);
                });

            migrationBuilder.CreateIndex(
                name: "ix_ms_users_role_id",
                table: "ms_users",
                column: "role_id");

            migrationBuilder.AddForeignKey(
                name: "fk_ms_users_ms_role_role_id",
                table: "ms_users",
                column: "role_id",
                principalTable: "ms_role",
                principalColumn: "id_role",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_ms_users_ms_role_role_id",
                table: "ms_users");

            migrationBuilder.DropTable(
                name: "ms_menu");

            migrationBuilder.DropTable(
                name: "ms_role");

            migrationBuilder.DropIndex(
                name: "ix_ms_users_role_id",
                table: "ms_users");
        }
    }
}
