public static class OscillationThresholds {
    // Ignore seeing-sized oscillations
    public static double MinimumAmplitudeArcSeconds { get; } = 0.20;

    // Diagnostic scoring
    public static double MediumCrossingRatePerMinute { get; } = 15.0;
    public static double HighCrossingRatePerMinute { get; } = 30.0;

    // Mean oscillation amplitude
    public static double MediumOscillationAmplitudeArcSeconds { get; } = 0.50;
    public static double HighOscillationAmplitudeArcSeconds { get; } = 1.00;
}