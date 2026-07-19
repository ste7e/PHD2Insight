namespace PHD2Insight.Analysis.Models;

public sealed record RmsResult {
    public double RaPixels { get; init; }

    public double DecPixels { get; init; }

    public double TotalPixels { get; init; }

    public double RaArcSeconds { get; init; }

    public double DecArcSeconds { get; init; }

    public double TotalArcSeconds { get; init; }
}