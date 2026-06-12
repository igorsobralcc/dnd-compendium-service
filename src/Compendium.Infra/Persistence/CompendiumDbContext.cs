using Compendium.Domain.Sources;
using Compendium.Infra.Persistence.Integration;
using Compendium.Infra.Persistence.Sources;
using Microsoft.EntityFrameworkCore;

namespace Compendium.Infra.Persistence;

public sealed class CompendiumDbContext : DbContext
{
    public const string Schema = "compendium";
    public const string MigrationsHistoryTable = "__ef_migrations_history";

    public CompendiumDbContext(DbContextOptions<CompendiumDbContext> options)
        : base(options)
    {
    }

    public DbSet<IntegrationOutbox> IntegrationOutbox => Set<IntegrationOutbox>();

    public DbSet<IntegrationOutboxField> IntegrationOutboxFields => Set<IntegrationOutboxField>();

    public DbSet<IntegrationInbox> IntegrationInbox => Set<IntegrationInbox>();

    public DbSet<Ruleset> Rulesets => Set<Ruleset>();

    public DbSet<RuleSource> RuleSources => Set<RuleSource>();

    public DbSet<SourceVersion> SourceVersions => Set<SourceVersion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfiguration(new IntegrationOutboxConfiguration());
        modelBuilder.ApplyConfiguration(new IntegrationOutboxFieldConfiguration());
        modelBuilder.ApplyConfiguration(new IntegrationInboxConfiguration());
        modelBuilder.ApplyConfiguration(new RulesetConfiguration());
        modelBuilder.ApplyConfiguration(new RuleSourceConfiguration());
        modelBuilder.ApplyConfiguration(new SourceVersionConfiguration());
    }
}
