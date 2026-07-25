using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Compendium.Infra.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSrdImportEpic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "source_version_imports",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    content_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    imported_entity_count = table.Column<int>(type: "integer", nullable: false),
                    imported_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_source_version_imports", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "source_version_validation_issues",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    severity = table.Column<int>(type: "integer", nullable: false),
                    message = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_source_version_validation_issues", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ux_source_version_imports_source_version_id",
                schema: "compendium",
                table: "source_version_imports",
                column: "source_version_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_source_version_validation_issues_source_version_id",
                schema: "compendium",
                table: "source_version_validation_issues",
                column: "source_version_id");

            migrationBuilder.CreateIndex(
                name: "ux_source_version_validation_issues_version_code",
                schema: "compendium",
                table: "source_version_validation_issues",
                columns: new[] { "source_version_id", "code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "source_version_imports",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "source_version_validation_issues",
                schema: "compendium");
        }
    }
}
