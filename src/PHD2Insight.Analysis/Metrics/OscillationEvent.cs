namespace PHD2Insight.Analysis.Metrics;

internal sealed record OscillationEvent(
    TimeSpan ElapsedTime,
    double PositiveAmplitudeArcSeconds,
    double NegativeAmplitudeArcSeconds) {
    public double MeanAmplitudeArcSeconds =>
        (PositiveAmplitudeArcSeconds + NegativeAmplitudeArcSeconds) / 2.0;
}