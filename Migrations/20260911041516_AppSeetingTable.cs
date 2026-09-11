using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace appOne.Migrations
{
    /// <inheritdoc />
    public partial class AppSeetingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "app_settings",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    app_name = table.Column<string>(type: "text", nullable: false),
                    app_short_name = table.Column<string>(type: "text", nullable: false),
                    app_tagline = table.Column<string>(type: "text", nullable: true),
                    app_logo = table.Column<string>(type: "text", nullable: true),
                    app_logo_small = table.Column<string>(type: "text", nullable: true),
                    favicon = table.Column<string>(type: "text", nullable: true),
                    primary_color = table.Column<string>(type: "text", nullable: true),
                    secondary_color = table.Column<string>(type: "text", nullable: true),
                    sidebar_color = table.Column<string>(type: "text", nullable: true),
                    navbar_color = table.Column<string>(type: "text", nullable: true),
                    footer_text = table.Column<string>(type: "text", nullable: true),
                    footer_license_url = table.Column<string>(type: "text", nullable: true),
                    footer_documentation_url = table.Column<string>(type: "text", nullable: true),
                    footer_support_url = table.Column<string>(type: "text", nullable: true),
                    version = table.Column<string>(type: "text", nullable: true),
                    environment = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_app_settings", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "app_settings");
        }
    }
}
