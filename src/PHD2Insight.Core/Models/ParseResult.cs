namespace PHD2Insight.Core.Models;

public sealed record ParseResult<T> {
    public bool Success { get; init; }

    public T? Value { get; init; }

    public IReadOnlyList<string> Errors { get; init; }
        = Array.Empty<string>();
}