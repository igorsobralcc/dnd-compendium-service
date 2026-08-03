using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Compendium.Application.Observability;

public static class CompendiumTelemetry
{
    public const string ServiceName = "dnd-compendium-service";
    public const string MeterName = ServiceName;
    public const string ActivitySourceName = ServiceName;

    public static readonly Meter Meter = new(MeterName, "1.0.0");
    public static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    public static readonly Histogram<double> HttpRequestDuration = Meter.CreateHistogram<double>(
        "compendium.http.server.request.duration", "ms", "HTTP request duration.");
    public static readonly Histogram<double> DatabaseQueryDuration = Meter.CreateHistogram<double>(
        "compendium.db.query.duration", "ms", "Database command duration.");
    public static readonly Counter<long> ImportFailures = Meter.CreateCounter<long>(
        "compendium.import.failures", "{failure}", "Failed source-version import requests.");
    public static readonly Counter<long> OutboxPublished = Meter.CreateCounter<long>(
        "compendium.outbox.published", "{event}", "Published integration events.");
    public static readonly Counter<long> OutboxPublicationFailures = Meter.CreateCounter<long>(
        "compendium.outbox.publication.failures", "{failure}", "Failed event publications.");
    public static readonly Counter<long> OutboxBacklogCollectionFailures = Meter.CreateCounter<long>(
        "compendium.outbox.backlog.collection.failures", "{failure}", "Failed Outbox backlog collections.");
    public static readonly Histogram<double> OutboxBacklogCollectionDuration = Meter.CreateHistogram<double>(
        "compendium.outbox.backlog.collection.duration", "ms", "Outbox backlog collection duration.");
    public static readonly Counter<long> OutboxCleanupDeleted = Meter.CreateCounter<long>(
        "compendium.outbox.cleanup.deleted", "{message}", "Published Outbox messages deleted by retention.");
    public static readonly Counter<long> OutboxCleanupFailures = Meter.CreateCounter<long>(
        "compendium.outbox.cleanup.failures", "{failure}", "Failed Outbox cleanup runs.");
    public static readonly Counter<long> OutboxExpiredClaimsRecovered = Meter.CreateCounter<long>(
        "compendium.outbox.claims.recovered", "{message}", "Expired Outbox claims recovered by a dispatcher.");

    private static long pendingOutboxMessages;

    public static readonly ObservableGauge<long> OutboxPending = Meter.CreateObservableGauge(
        "compendium.outbox.pending",
        () => Interlocked.Read(ref pendingOutboxMessages),
        "{message}",
        "Deprecated alias for unresolved Outbox messages.");

    public static readonly ObservableGauge<long> OutboxUnresolved = Meter.CreateObservableGauge(
        "compendium.outbox.unresolved",
        () => Interlocked.Read(ref pendingOutboxMessages),
        "{message}",
        "Outbox messages not yet published or dead-lettered.");

    public static void SetPendingOutboxMessages(long value) =>
        Interlocked.Exchange(ref pendingOutboxMessages, value);
}
