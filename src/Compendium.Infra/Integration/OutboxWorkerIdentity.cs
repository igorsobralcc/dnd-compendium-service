namespace Compendium.Infra.Integration;

internal sealed class OutboxWorkerIdentity
{
    public OutboxWorkerIdentity()
    {
        var value = $"{Environment.MachineName}:{Environment.ProcessId}:{Guid.NewGuid():N}";
        Value = value.Length <= 128 ? value : value[..128];
    }

    public string Value { get; }
}
