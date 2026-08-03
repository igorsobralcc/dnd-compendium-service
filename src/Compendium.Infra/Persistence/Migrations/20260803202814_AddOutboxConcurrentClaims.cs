using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Compendium.Infra.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOutboxConcurrentClaims : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "claim_token",
                schema: "compendium",
                table: "integration_outbox",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "lease_expires_at_utc",
                schema: "compendium",
                table: "integration_outbox",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "processing_owner",
                schema: "compendium",
                table: "integration_outbox",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "processing_started_at_utc",
                schema: "compendium",
                table: "integration_outbox",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.Sql(
                """
                CREATE INDEX CONCURRENTLY ix_integration_outbox_processing_lease
                ON compendium.integration_outbox (lease_expires_at_utc, created_at_utc)
                WHERE status = 'PROCESSING';
                """,
                suppressTransaction: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DROP INDEX CONCURRENTLY IF EXISTS compendium.ix_integration_outbox_processing_lease;",
                suppressTransaction: true);

            migrationBuilder.DropColumn(
                name: "claim_token",
                schema: "compendium",
                table: "integration_outbox");

            migrationBuilder.DropColumn(
                name: "lease_expires_at_utc",
                schema: "compendium",
                table: "integration_outbox");

            migrationBuilder.DropColumn(
                name: "processing_owner",
                schema: "compendium",
                table: "integration_outbox");

            migrationBuilder.DropColumn(
                name: "processing_started_at_utc",
                schema: "compendium",
                table: "integration_outbox");
        }
    }
}
