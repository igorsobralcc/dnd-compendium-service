using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Compendium.Infra.Persistence.Migrations;

[DbContext(typeof(CompendiumDbContext))]
[Migration("20260803090000_AddOutboxPerformanceIndexes")]
public sealed class AddOutboxPerformanceIndexes : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            CREATE INDEX CONCURRENTLY ix_integration_outbox_active_available_created
            ON compendium.integration_outbox (available_at_utc, created_at_utc)
            WHERE status IN ('PENDING', 'FAILED');
            """,
            suppressTransaction: true);
        migrationBuilder.Sql(
            """
            CREATE INDEX CONCURRENTLY ix_integration_outbox_published_at
            ON compendium.integration_outbox (published_at_utc)
            WHERE status = 'PUBLISHED';
            """,
            suppressTransaction: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            "DROP INDEX CONCURRENTLY IF EXISTS compendium.ix_integration_outbox_published_at;",
            suppressTransaction: true);
        migrationBuilder.Sql(
            "DROP INDEX CONCURRENTLY IF EXISTS compendium.ix_integration_outbox_active_available_created;",
            suppressTransaction: true);
    }
}
