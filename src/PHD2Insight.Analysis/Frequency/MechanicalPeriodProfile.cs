namespace PHD2Insight.Analysis.Frequency;

public sealed record MountPeriodProfile(
    string Name,
    IReadOnlyList<MechanicalPeriod> RaPeriods,
    IReadOnlyList<MechanicalPeriod> DecPeriods);