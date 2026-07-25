using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Compendium.Infra.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddInternalQueryApisEpic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "compendium_changes",
                schema: "compendium",
                columns: table => new
                {
                    revision = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    source_version_id = table.Column<Guid>(type: "uuid", nullable: true),
                    entity_type = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    entity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    change_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    changed_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_compendium_changes", x => x.revision);
                });

            migrationBuilder.CreateIndex(
                name: "ix_compendium_changes_changed_at",
                schema: "compendium",
                table: "compendium_changes",
                column: "changed_at_utc");

            migrationBuilder.CreateIndex(
                name: "ix_compendium_changes_source_revision",
                schema: "compendium",
                table: "compendium_changes",
                columns: new[] { "source_version_id", "revision" });

            migrationBuilder.CreateIndex(
                name: "ix_compendium_changes_type_revision",
                schema: "compendium",
                table: "compendium_changes",
                columns: new[] { "entity_type", "revision" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "compendium_changes",
                schema: "compendium");
        }
    }
}
