namespace PHD2Insight.Analysis.Models;

public sealed record GuideCorrectionResult {
    public int RaCorrectionCount { get; init; }

    public int DecCorrectionCount { get; init; }

    public double AverageRaPulseMilliseconds { get; init; }

    public double AverageDecPulseMilliseconds { get; init; }

    public double MaximumRaPulseMilliseconds { get; init; }

    public double MaximumDecPulseMilliseconds { get; init; }

    public TimeSpan TotalRaCorrectionTime { get; init; }

    public TimeSpan TotalDecCorrectionTime { get; init; }
}