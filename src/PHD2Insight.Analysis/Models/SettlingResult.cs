public sealed record SettlingResult {
    public int SettlingAttemptCount { get; init; }

    public int SuccessfulSettles { get; init; }

    public int FailedSettles { get; init; }

    public int CancelledSettles { get; init; }

    public TimeSpan AverageSettlingTime { get; init; }

    public TimeSpan LongestSettlingTime { get; init; }

    public TimeSpan ShortestSettlingTime { get; init; }
}