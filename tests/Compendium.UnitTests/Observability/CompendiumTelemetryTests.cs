using System.Diagnostics.Metrics;
using Compendium.Application.Observability;

namespace Compendium.UnitTests.Observability;

public sealed class CompendiumTelemetryTests
{
    [Fact]
    public void Application_metrics_are_observable_without_infrastructure()
    {
        var measurements = new List<(string Name, double Value)>();
        using var listener = new MeterListener();
        listener.InstrumentPublished = (instrument, meterListener) =>
        {
            if (instrument.Meter.Name == CompendiumTelemetry.MeterName)
                meterListener.EnableMeasurementEvents(instrument);
        };
        listener.SetMeasurementEventCallback<double>((instrument, value, _, _) =>
            measurements.Add((instrument.Name, value)));
        listener.Start();

        CompendiumTelemetry.HttpRequestDuration.Record(12.5);

        Assert.Contains(measurements, item =>
            item.Name == "compendium.http.server.request.duration" && item.Value == 12.5);
    }
}
