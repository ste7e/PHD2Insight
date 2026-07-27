namespace PHD2Insight.Analysis.Metrics;

internal sealed record OscillationEvent(
    TimeSpan ElapsedTime,
    double PreviousValue,
    double CurrentValue) {
    public double MeanAmplitude =>
        (Math.Abs(PreviousValue) +
         Math.Abs(CurrentValue)) / 2.0;
}
