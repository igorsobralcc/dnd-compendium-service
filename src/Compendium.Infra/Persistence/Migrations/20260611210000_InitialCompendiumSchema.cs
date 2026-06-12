using Compendium.Infra.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Compendium.Infra.Persistence.Migrations;

[DbContext(typeof(CompendiumDbContext))]
[Migration("20260611210000_InitialCompendiumSchema")]
public partial class InitialCompendiumSchema : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(name: CompendiumDbContext.Schema);

        migrationBuilder.CreateTable(
            name: "integration_inbox",
            schema: CompendiumDbContext.Schema,
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                event_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                consumer_name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                event_name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                event_version = table.Column<int>(type: "integer", nullable: false),
                correlation_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                received_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                processed_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                status = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                retry_count = table.Column<int>(type: "integer", nullable: false),
                last_error = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_integration_inbox", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "integration_outbox",
            schema: CompendiumDbContext.Schema,
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                event_id = table.Column<Guid>(type: "uuid", nullable: false),
                event_name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                event_version = table.Column<int>(type: "integer", nullable: false),
                aggregate_type = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                aggregate_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                correlation_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                occurred_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                available_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                published_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                status = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                retry_count = table.Column<int>(type: "integer", nullable: false),
                last_error = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_integration_outbox", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "integration_outbox_fields",
            schema: CompendiumDbContext.Schema,
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                outbox_id = table.Column<Guid>(type: "uuid", nullable: false),
                field_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                field_type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                text_value = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                number_value = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                boolean_value = table.Column<bool>(type: "boolean", nullable: true),
                reference_value = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                enum_value = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_integration_outbox_fields", x => x.id);
                table.ForeignKey(
                    name: "fk_integration_outbox_fields_outbox",
                    column: x => x.outbox_id,
                    principalSchema: CompendiumDbContext.Schema,
                    principalTable: "integration_outbox",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_integration_inbox_status",
            schema: CompendiumDbContext.Schema,
            table: "integration_inbox",
            column: "status");

        migrationBuilder.CreateIndex(
            name: "ux_integration_inbox_event_consumer",
            schema: CompendiumDbContext.Schema,
            table: "integration_inbox",
            columns: new[] { "event_id", "consumer_name" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_integration_outbox_aggregate_id",
            schema: CompendiumDbContext.Schema,
            table: "integration_outbox",
            column: "aggregate_id");

        migrationBuilder.CreateIndex(
            name: "ix_integration_outbox_status",
            schema: CompendiumDbContext.Schema,
            table: "integration_outbox",
            column: "status");

        migrationBuilder.CreateIndex(
            name: "ux_integration_outbox_event_id",
            schema: CompendiumDbContext.Schema,
            table: "integration_outbox",
            column: "event_id",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_integration_outbox_fields_outbox_field",
            schema: CompendiumDbContext.Schema,
            table: "integration_outbox_fields",
            columns: new[] { "outbox_id", "field_name" });

        migrationBuilder.CreateIndex(
            name: "ix_integration_outbox_fields_outbox_id",
            schema: CompendiumDbContext.Schema,
            table: "integration_outbox_fields",
            column: "outbox_id");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "integration_inbox",
            schema: CompendiumDbContext.Schema);

        migrationBuilder.DropTable(
            name: "integration_outbox_fields",
            schema: CompendiumDbContext.Schema);

        migrationBuilder.DropTable(
            name: "integration_outbox",
            schema: CompendiumDbContext.Schema);
    }
}
