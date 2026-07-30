internal sealed record OscillationEvent(
    TimeSpan StartTime,
    TimeSpan EndTime,
    double PositivePeakArcSeconds,
    double NegativePeakArcSeconds) {
    public double PeakToPeakAmplitudeArcSeconds =>
        PositivePeakArcSeconds + Math.Abs(NegativePeakArcSeconds);

    public double MeanAmplitudeArcSeconds =>
        PeakToPeakAmplitudeArcSeconds / 2.0;

    public TimeSpan Period => EndTime - StartTime;
}
