public sealed record GuideReversalResult {
    public int RaReversalCount { get; init; }

    public int DecReversalCount { get; init; }

    public double RaReversalRatePerMinute { get; init; }

    public double DecReversalRatePerMinute { get; init; }
}