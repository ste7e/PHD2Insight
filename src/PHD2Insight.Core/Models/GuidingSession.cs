namespace PHD2Insight.Core.Models;

/// <summary>
/// Represents one guiding run.
/// </summary>
public sealed record GuidingSession {
    public DateTime StartTime { get; init; }

    public DateTime? EndTime { get; init; }

    public EquipmentProfile? Equipment { get; init; }

    public IReadOnlyList<GuideSample> Samples { get; init; }
        = Array.Empty<GuideSample>();
}