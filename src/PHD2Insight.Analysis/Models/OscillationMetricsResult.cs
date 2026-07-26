namespace PHD2Insight.Analysis.Models;

public sealed record OscillationMetricsResult {
    public double MeanRaErrorArcSeconds { get; init; }

    public double MeanDecErrorArcSeconds { get; init; }

    public double MeanAbsoluteRaErrorArcSeconds { get; init; }

    public double MeanAbsoluteDecErrorArcSeconds { get; init; }

    public double StandardDeviationRaErrorArcSeconds { get; init; }

    public double StandardDeviationDecErrorArcSeconds { get; init; }

    public int RaZeroCrossings { get; init; }

    public int DecZeroCrossings { get; init; }

    public int RaDirectionReversals { get; init; }

    public int DecDirectionReversals { get; init; }

    public double RaZeroCrossingsPerMinute { get; init; }

    public double RaDirectionChangesPerMinute { get; init; }

    public double DecZeroCrossingsPerMinute { get; init; }

    public double DecDirectionChangesPerMinute { get; init; }

    public double MeanRaOscillationAmplitude { get; init; }

    public double MeanDecOscillationAmplitude { get; init; }
}