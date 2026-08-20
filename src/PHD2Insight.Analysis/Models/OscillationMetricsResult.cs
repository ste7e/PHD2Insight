using PHD2Insight.Analysis.Frequency;

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

    public double? RaDominantFrequencyHz { get; init; }

    public double? RaDominantPeriodSeconds { get; init; }

    public double? RaFrequencyPower { get; init; }

    public double? DecDominantFrequencyHz { get; init; }

    public double? DecDominantPeriodSeconds { get; init; }

    public double? DecFrequencyPower { get; init; }


    public IReadOnlyList<MechanicalPeriodAnalysisResult> RaMechanicalPeriods { get; init; }
    = Array.Empty<MechanicalPeriodAnalysisResult>();

    public IReadOnlyList<MechanicalPeriodAnalysisResult> DecMechanicalPeriods { get; init; }
        = Array.Empty<MechanicalPeriodAnalysisResult>();

    public MechanicalPeriodPowerResult MechanicalPeriodPower { get; init; } = new();
}