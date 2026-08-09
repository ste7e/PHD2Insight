using PHD2Insight.Analysis.Diagnostics;
using PHD2Insight.Analysis.Models;
using PHD2Insight.Analysis.Observations;

public sealed class GuideReversalDiagnosisRule
    : DiagnosticRuleBase {

    private static readonly HashSet<string> EvidenceObservationCodes = [
        ObservationCodes.RaGuideReversal,
        ObservationCodes.DecGuideReversal
    ];

    public override IEnumerable<Diagnosis> Evaluate(
        AnalysisResult analysis) {

        var diagnosis = BuildDiagnosis(
            analysis,
            DiagnosisCodes.GuideReversal,
            "Guide Correction Reversals",
            "Guide corrections frequently reverse direction, indicating that the guiding system may be over-correcting.",
            DiagnosisSeverity.Warning,
            EvidenceObservationCodes);

        if (diagnosis != null) {
            yield return diagnosis;
        }
    }
}