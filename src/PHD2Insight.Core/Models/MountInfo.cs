namespace PHD2Insight.Core.Models;

public sealed record MountInfo {
    public string Name { get; init; } = string.Empty;

    public bool Connected { get; init; }

    public bool GuidingEnabled { get; init; }

    public double XAngleDegrees { get; init; }

    public double XRate { get; init; }

    public double YAngleDegrees { get; init; }

    public double YRate { get; init; }

    public string? Parity { get; init; }
}