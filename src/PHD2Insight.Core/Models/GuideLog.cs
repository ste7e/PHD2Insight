namespace PHD2Insight.Core.Models;

/// <summary>
/// Represents the contents of a single PHD2 guide log.
/// </summary>
public sealed record GuideLog {
    public string Phd2Version { get; init; } = string.Empty;

    public string LogVersion { get; init; } = string.Empty;

    public IReadOnlyList<GuidingSession> Sessions { get; init; }
        = Array.Empty<GuidingSession>();
}