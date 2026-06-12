using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Compendium.Infra.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSourcesRulesetsAndVersions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "rule_sources",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ruleset_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    name = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    status = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rule_sources", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "rulesets",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    version = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    status = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rulesets", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "source_versions",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    rule_source_id = table.Column<Guid>(type: "uuid", nullable: false),
                    version_number = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    publication_date = table.Column<DateOnly>(type: "date", nullable: false),
                    import_status = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    is_current = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_source_versions", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_rule_sources_ruleset_id",
                schema: "compendium",
                table: "rule_sources",
                column: "ruleset_id");

            migrationBuilder.CreateIndex(
                name: "ux_rule_sources_ruleset_code",
                schema: "compendium",
                table: "rule_sources",
                columns: new[] { "ruleset_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_rulesets_code",
                schema: "compendium",
                table: "rulesets",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_source_versions_rule_source_id",
                schema: "compendium",
                table: "source_versions",
                column: "rule_source_id");

            migrationBuilder.CreateIndex(
                name: "ux_source_versions_current_per_source",
                schema: "compendium",
                table: "source_versions",
                columns: new[] { "rule_source_id", "is_current" },
                unique: true,
                filter: "is_current = true");

            migrationBuilder.CreateIndex(
                name: "ux_source_versions_source_version",
                schema: "compendium",
                table: "source_versions",
                columns: new[] { "rule_source_id", "version_number" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rule_sources",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "rulesets",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "source_versions",
                schema: "compendium");
        }
    }
}
