namespace PHD2Insight.Analysis.Recommendations;

public sealed record Recommendation {
    public required string Code { get; init; }

    public required string Title { get; init; }

    public required string Description { get; init; }

    public RecommendationPriority Priority { get; init; }

    public IReadOnlyList<string> SupportingDiagnosisCodes { get; init; } = Array.Empty<string>();

    public IReadOnlyList<string> SupportingObservationCodes { get; init; } = Array.Empty<string>();
}