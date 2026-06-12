using Compendium.Domain.SharedKernel;

namespace Compendium.Application;

internal sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
