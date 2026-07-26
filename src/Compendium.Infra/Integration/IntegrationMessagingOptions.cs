namespace Compendium.Infra.Integration;

public sealed class IntegrationMessagingOptions
{
    public const string SectionName = "IntegrationMessaging";

    public TimeSpan PollingInterval { get; set; } = TimeSpan.FromSeconds(2);
    public int BatchSize { get; set; } = 50;
    public int MaxRetries { get; set; } = 5;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(10);
}
