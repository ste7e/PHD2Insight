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

    // TODO: Determine the semantics of IsGuiding once DROP records and
    // other non-Mount frame types are supported.
    public bool IsGuiding { get; init; }

    public double StarMass { get; init; }

    public double SignalToNoiseRatio { get; init; }

    public int ErrorCode { get; init; }

    public double RaGuideDistance { get; init; }

    public double DecGuideDistance { get; init; }

    public GuideDirection RaDirection { get; init; }

    public GuideDirection DecDirection { get; init; }
}