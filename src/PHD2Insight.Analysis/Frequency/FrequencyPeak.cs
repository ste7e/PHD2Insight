namespace PHD2Insight.Analysis.Frequency;

public sealed record FrequencyPeak(
    double PeriodSeconds,
    double Power,
    MechanicalPeriodMatch? MechanicalMatch = null);