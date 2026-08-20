namespace PHD2Insight.Analysis.Frequency;

public sealed record MechanicalPeriod(
    string Name,
    double PeriodSeconds,
    int? Harmonic = null);