namespace PHD2Insight.Analysis.Models;

public sealed record PeakErrorResult {
    public double MaximumRaErrorPixels { get; init; }

    public double MaximumDecErrorPixels { get; init; }

    public double MaximumTotalErrorPixels { get; init; }

    public double MaximumRaErrorArcSeconds { get; init; }

    public double MaximumDecErrorArcSeconds { get; init; }

    public double MaximumTotalErrorArcSeconds { get; init; }
}