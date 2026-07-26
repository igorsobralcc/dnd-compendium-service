namespace Compendium.Infra.Persistence.Integration;

public static class IntegrationMessageStatus
{
    public const string Pending = "PENDING";
    public const string Published = "PUBLISHED";
    public const string Failed = "FAILED";
    public const string DeadLetter = "DEAD_LETTER";
    public const string Processing = "PROCESSING";
    public const string Received = "RECEIVED";
    public const string Processed = "PROCESSED";
}
