namespace PHD2Insight.Analysis.Models;

public sealed record SessionStatisticsResult {
    public int FrameCount { get; init; }

    public TimeSpan? Duration { get; init; }

    public double AverageSignalToNoiseRatio { get; init; }

    public double AverageStarMass { get; init; }
}