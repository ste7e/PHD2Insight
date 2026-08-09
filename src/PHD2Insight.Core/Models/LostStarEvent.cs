namespace PHD2Insight.Parser.Models;

public sealed record LostStarEvent {
    public required TimeSpan ElapsedTime { get; init; }

    public required int ErrorCode { get; init; }

    public required string ErrorMessage { get; init; }

    public bool IsStarLost => ErrorCode == 7;
}