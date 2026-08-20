namespace PHD2Insight.Analysis.Frequency;

public sealed record MechanicalPeriodMatch(
    MechanicalPeriod Period,
    int Harmonic,
    double ExpectedPeriodSeconds,
    double MeasuredPeriodSeconds,
    double ErrorPercent);