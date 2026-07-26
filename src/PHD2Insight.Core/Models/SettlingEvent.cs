namespace PHD2Insight.Core.Models;

public sealed record SettlingEvent {
    public TimeSpan ElapsedTime { get; init; }

    public SettlingState State { get; init; }

    public double StartElapsedTime { get; init; }

    public double EndElapsedTime { get; init; }
}