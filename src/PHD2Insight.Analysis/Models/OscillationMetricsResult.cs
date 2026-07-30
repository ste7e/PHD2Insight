namespace PHD2Insight.Analysis.Models;

public sealed record OscillationMetricsResult {
    public double MeanRaErrorArcSeconds { get; init; }

    public double MeanDecErrorArcSeconds { get; init; }

    public double MeanAbsoluteRaErrorArcSeconds { get; init; }

    public double MeanAbsoluteDecErrorArcSeconds { get; init; }

    public double StandardDeviationRaErrorArcSeconds { get; init; }

    public double StandardDeviationDecErrorArcSeconds { get; init; }

    public int RaDirectionReversals { get; init; }

    public int DecDirectionReversals { get; init; }

    public double RaOscillationEventsPerMinute { get; init; }

    public double RaDirectionChangesPerMinute { get; init; }

    public double DecOscillationEventsPerMinute { get; init; }

    public double DecDirectionChangesPerMinute { get; init; }

    public double MeanRaOscillationAmplitudeArcSeconds { get; init; }

    public double MeanDecOscillationAmplitudeArcSeconds { get; init; }
}