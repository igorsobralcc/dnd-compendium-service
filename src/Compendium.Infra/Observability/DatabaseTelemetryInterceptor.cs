using System.Data.Common;
using Compendium.Application.Observability;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Compendium.Infra.Observability;

internal sealed class DatabaseTelemetryInterceptor : DbCommandInterceptor
{
    public override DbDataReader ReaderExecuted(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result)
    {
        Record(eventData);
        return result;
    }

    public override ValueTask<DbDataReader> ReaderExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result,
        CancellationToken cancellationToken = default)
    {
        Record(eventData);
        return ValueTask.FromResult(result);
    }

    public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
    {
        Record(eventData);
        return result;
    }

    public override object? ScalarExecuted(DbCommand command, CommandExecutedEventData eventData, object? result)
    {
        Record(eventData);
        return result;
    }

    private static void Record(CommandExecutedEventData eventData) =>
        CompendiumTelemetry.DatabaseQueryDuration.Record(
            eventData.Duration.TotalMilliseconds,
            new KeyValuePair<string, object?>("db.system", "postgresql"),
            new KeyValuePair<string, object?>("db.operation", eventData.CommandSource.ToString()));
}
