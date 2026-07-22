namespace PHD2Insight.Analysis.Diagnostics;

public sealed record Diagnosis {
    public string Code { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public DiagnosisSeverity Severity { get; init; }

    public DiagnosisConfidence Confidence { get; init; }

    public IReadOnlyList<DiagnosisEvidence> Evidence { get; init; }
        = Array.Empty<DiagnosisEvidence>();
}