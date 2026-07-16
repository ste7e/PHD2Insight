namespace PHD2Insight.Core.Models;

public sealed record GuideAlgorithmInfo {
    public string Name { get; init; } = string.Empty;

    public double? MinimumMove { get; init; }

    public double? Aggression { get; init; }

    public double? ControlGain { get; init; }

    public double? PredictionGain { get; init; }

    public bool? FastSwitch { get; init; }
}