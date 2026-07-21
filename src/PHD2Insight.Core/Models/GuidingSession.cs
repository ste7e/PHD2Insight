namespace PHD2Insight.Core.Models;

/// <summary>
/// Represents one guiding run.
/// </summary>
public sealed record GuidingSession {
    public DateTime StartTime { get; init; }

    public DateTime? EndTime { get; init; }

    public EquipmentProfile? Equipment { get; init; }

    public int ExposureMilliseconds { get; init; }

    public double PixelScale { get; init; }

    public int Binning { get; init; }

    public int FocalLengthMm { get; init; }

    public CameraInfo? Camera { get; init; }

    public MountInfo? Mount { get; init; }

    public GuideAlgorithmInfo? XGuideAlgorithm { get; init; }

    public GuideAlgorithmInfo? YGuideAlgorithm { get; init; }

    public GuideFrameSchema? GuideFrameSchema { get; init; }

    public ICollection<GuideFrame> Frames { get; init; } = Array.Empty<GuideFrame>();

    public ICollection<SettlingEvent> SettlingEvents { get; init; } = Array.Empty<SettlingEvent>();

}