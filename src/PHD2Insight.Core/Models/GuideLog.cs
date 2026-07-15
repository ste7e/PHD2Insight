namespace PHD2Insight.Core.Models;

/// <summary>
/// Represents the contents of a single PHD2 guide log.
/// </summary>
public sealed record GuideLog {
    /// <summary>
    /// Gets the PHD2 and log format version information.
    /// </summary>
    public GuideLogVersion? Version { get; init; }

    /// <summary>
    /// Gets the guiding sessions contained in this log.
    /// </summary>
    public IReadOnlyList<GuidingSession> Sessions { get; init; }
        = Array.Empty<GuidingSession>();
}