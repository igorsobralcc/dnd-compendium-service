namespace Compendium.Domain.SharedKernel;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
