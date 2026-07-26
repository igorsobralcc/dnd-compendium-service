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

    private static long pendingOutboxMessages;

    public static readonly ObservableGauge<long> OutboxPending = Meter.CreateObservableGauge(
        "compendium.outbox.pending",
        () => Interlocked.Read(ref pendingOutboxMessages),
        "{message}",
        "Pending or retryable Outbox messages observed by the dispatcher.");

    public static void SetPendingOutboxMessages(long value) =>
        Interlocked.Exchange(ref pendingOutboxMessages, value);
}
