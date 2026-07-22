namespace PHD2Insight.Analysis.Diagnostics;

public sealed record DiagnosisEvidence {
    public string Metric { get; init; } = string.Empty;

    public string Value { get; init; } = string.Empty;

    public string Explanation { get; init; } = string.Empty;
}