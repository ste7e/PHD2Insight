using PHD2Insight.Common.Enums;

namespace PHD2Insight.Core.Models;

public sealed record GuideSample {
    public int FrameNumber { get; init; }

    public double ElapsedSeconds { get; init; }

    public double Dx { get; init; }

    public double Dy { get; init; }

    public double RaRawDistance { get; init; }

    public double DecRawDistance { get; init; }

    public double RaGuideDistance { get; init; }

    public double DecGuideDistance { get; init; }

    public int RaDuration { get; init; }

    public GuideDirection RaDirection { get; init; }

    public int DecDuration { get; init; }

    public GuideDirection DecDirection { get; init; }

    public double StarMass { get; init; }

    public double Snr { get; init; }

    public int ErrorCode { get; init; }
}