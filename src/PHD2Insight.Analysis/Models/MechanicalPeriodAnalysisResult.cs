namespace PHD2Insight.Analysis.Frequency;

public sealed record MechanicalPeriodAnalysisResult(
    MechanicalPeriod Period,
    double MeasuredPeriodSeconds,
    double FrequencyHz,
    double Power);