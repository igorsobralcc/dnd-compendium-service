using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Compendium.Infra.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTranslationsEpic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "translations",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    entity_type = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    entity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    locale = table.Column<string>(type: "character varying(35)", maxLength: 35, nullable: false),
                    field = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    translated_text = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_translations", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_translations_entity",
                schema: "compendium",
                table: "translations",
                columns: new[] { "entity_type", "entity_id" });

            migrationBuilder.CreateIndex(
                name: "ux_translations_entity_locale_field",
                schema: "compendium",
                table: "translations",
                columns: new[] { "entity_type", "entity_id", "locale", "field" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "translations",
                schema: "compendium");
        }
    }
}
