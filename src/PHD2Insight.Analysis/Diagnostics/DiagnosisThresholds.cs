internal static class DiagnosisThresholds {
    public const double HighRaRmsArcSeconds = 1.2;
    public const double HighDecRmsArcSeconds = 1.2;

    public const double HighRaToDecRatio = 2.0;
    public const double HighDecToRaRatio = 2.0;

    public const double MediumRaOscillationEventsPerMinute = 5.0;
    public const double HighRaOscillationEventsPerMinute = 14.0;

    public const double MediumDecOscillationEventsPerMinute = 1.0;
    public const double HighDecOscillationEventsPerMinute = 9.0;

    public const int HighRaDirectionReversals = 150;

    public const double LargeAverageRaPulseMilliseconds = 200;

    public const double MediumRaOscillationAmplitudeArcSeconds = 0.90;
    public const double HighRaOscillationAmplitudeArcSeconds = 2.00;

    public const double MediumDecOscillationAmplitudeArcSeconds = 0.70;
    public const double HighDecOscillationAmplitudeArcSeconds = 2.00;

    public const int MinimumDiagnosisScore = 4;

    public const double LargeAverageDecPulseMilliseconds = 200;

    public const double FrequentLostStarPercentage = 2.0;

    public const double SevereLostStarPercentage = 10.0;

    public const double LowRaOscillationAmplitudeArcSeconds = 0.50;

    public const double LowDecOscillationAmplitudeArcSeconds = 0.50;

    public const double NormalAverageGuidePulseMilliseconds = 100.0;

    public const double FrequentRaGuideReversalRatePerMinute = 7.5;
    public const double FrequentDecGuideReversalRatePerMinute = 2.5;
}