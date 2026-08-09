namespace PHD2Insight.Analysis.Observations;

public sealed record Observation {
    public required string Code { get; init; }

    public required string Metric { get; init; }

    public required string Value { get; init; }

    public required string Description { get; init; }

    public required int Weight { get; init; }
}