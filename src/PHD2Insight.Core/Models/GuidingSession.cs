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

    public int ExposureMilliseconds { get; init; }

    public double PixelScale { get; init; }

    public int Binning { get; init; }

    public int FocalLengthMm { get; init; }

    public CameraInfo? Camera { get; init; }
}