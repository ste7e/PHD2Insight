namespace PHD2Insight.Core.Models;

public sealed class GuideSample {
    public DateTime Timestamp { get; init; }

    /// <summary>
    /// RA error in arc-seconds.
    /// Positive = east error.
    /// </summary>
    public double RaError { get; init; }


    /// <summary>
    /// Declination error in arc-seconds.
    /// Positive = north error.
    /// </summary>
    public double DecError { get; init; }


    /// <summary>
    /// RA guide pulse duration in milliseconds.
    /// </summary>
    public int RaPulseMs { get; init; }


    /// <summary>
    /// DEC guide pulse duration in milliseconds.
    /// </summary>
    public int DecPulseMs { get; init; }
}