namespace Compendium.Infra.Integration;

public sealed class IntegrationMessagingOptions
{
    public const string SectionName = "IntegrationMessaging";

    public TimeSpan PollingInterval { get; set; } = TimeSpan.FromSeconds(2);
    public TimeSpan BacklogMetricsInterval { get; set; } = TimeSpan.FromMinutes(1);
    public int BatchSize { get; set; } = 50;
    public int MaxRetries { get; set; } = 5;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(10);
    public TimeSpan ProcessingLeaseDuration { get; set; } = TimeSpan.FromMinutes(2);
    public TimeSpan PublishAttemptTimeout { get; set; } = TimeSpan.FromSeconds(30);
    public bool CleanupEnabled { get; set; }
    public TimeSpan PublishedRetention { get; set; } = TimeSpan.FromDays(30);
    public TimeSpan CleanupInterval { get; set; } = TimeSpan.FromHours(1);
    public int CleanupBatchSize { get; set; } = 1_000;
    public int CleanupMaxBatchesPerRun { get; set; } = 10;
    public TimeSpan CleanupInterBatchDelay { get; set; } = TimeSpan.FromMilliseconds(100);
}
