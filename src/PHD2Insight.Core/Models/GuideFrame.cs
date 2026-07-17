namespace PHD2Insight.Core.Models;

public sealed class GuideFrame {
    public int FrameNumber { get; init; }

    public TimeSpan ElapsedTime { get; init; }

    public double RaErrorPixels { get; init; }

    public double DecErrorPixels { get; init; }

    public double RaErrorArcSeconds { get; init; }

    public double DecErrorArcSeconds { get; init; }

    public double? RaPulseMilliseconds { get; init; }

    public double? DecPulseMilliseconds { get; init; }

    public bool IsGuiding { get; init; }
}