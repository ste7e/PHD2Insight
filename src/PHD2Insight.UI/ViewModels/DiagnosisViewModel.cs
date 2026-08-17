using PHD2Insight.Analysis.Diagnostics;

public sealed class DiagnosisViewModel {
    private readonly Diagnosis _diagnosis;

    public DiagnosisViewModel(Diagnosis diagnosis) {
        _diagnosis = diagnosis;
    }

    public string Title => _diagnosis.Title;

    public string Description => _diagnosis.Description;

    public DiagnosisSeverity Severity => _diagnosis.Severity;

    public DiagnosisConfidence Confidence => _diagnosis.Confidence;

    public string SeverityForeground =>
        Severity switch {
            DiagnosisSeverity.Information => "Gray",
            DiagnosisSeverity.Warning => "DarkOrange",
            DiagnosisSeverity.Critical => "Red",
            _ => "Gray"
        };

    public string ConfidenceForeground =>
        Confidence switch {
            DiagnosisConfidence.High => "Green",
            DiagnosisConfidence.Medium => "DarkOrange",
            DiagnosisConfidence.Low => "Gray",
            _ => "Gray"
        };
}