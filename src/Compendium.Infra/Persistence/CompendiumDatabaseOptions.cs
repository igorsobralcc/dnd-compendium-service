namespace Compendium.Infra.Persistence;

public sealed class CompendiumDatabaseOptions
{
    public const string SectionName = "Compendium:Database";
    public const string ConnectionStringName = "CompendiumDb";
    public const string DefaultLocalConnectionString =
        "Host=localhost;Port=5432;Database=compendium;Username=compendium;Password=compendium";

    public string Schema { get; init; } = CompendiumDbContext.Schema;
}
