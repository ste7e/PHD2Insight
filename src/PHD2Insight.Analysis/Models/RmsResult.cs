namespace PHD2Insight.Analysis.Models;

public sealed record RmsResult {
    public double RaPixels { get; init; }

    public double DecPixels { get; init; }

    public double TotalPixels { get; init; }

    public double RaArcSeconds { get; init; }

    public double DecArcSeconds { get; init; }

    public double RaToDecRatio { get; init; }

    public double TotalArcSeconds { get; init; }

    public double MeanRaPixels { get; init; }

    public double MeanDecPixels { get; init; }

    public double MeanRaArcSeconds { get; init; }

    public double MeanDecArcSeconds { get; init; }

    public double MeanRadialOffsetArcSeconds =>
                    System.Math.Sqrt(
                        MeanRaArcSeconds * MeanRaArcSeconds +
                        MeanDecArcSeconds * MeanDecArcSeconds);
}