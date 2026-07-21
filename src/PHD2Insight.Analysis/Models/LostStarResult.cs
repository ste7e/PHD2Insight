namespace PHD2Insight.Analysis.Models;

public sealed record LostStarResult {
    public int LostStarCount { get; init; }

    public double LostStarPercentage { get; init; }

    public TimeSpan? FirstOccurrence { get; init; }

    public TimeSpan? LastOccurrence { get; init; }

    public IReadOnlyDictionary<string, int> ErrorMessages { get; init; }
        = new Dictionary<string, int>();
}