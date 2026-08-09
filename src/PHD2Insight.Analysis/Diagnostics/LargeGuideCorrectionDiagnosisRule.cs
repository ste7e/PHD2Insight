using PHD2Insight.Analysis.Models;
using PHD2Insight.Analysis.Observations;

namespace PHD2Insight.Analysis.Diagnostics;

public sealed class LargeGuideCorrectionsDiagnosisRule
    : DiagnosticRuleBase {

    private static readonly HashSet<string> EvidenceObservationCodes = [
        ObservationCodes.LargeRaGuidePulses,
        ObservationCodes.LargeDecGuidePulses
    ];

    public override IEnumerable<Diagnosis> Evaluate(
        AnalysisResult analysis) {

        var diagnosis = BuildDiagnosis(
            analysis,
            DiagnosisCodes.LargeGuideCorrections,
            "Large Guide Corrections",
            "Guide corrections are consistently large on both axes, indicating that the guiding system may be making excessive corrections.",
            DiagnosisSeverity.Warning,
            EvidenceObservationCodes,
            (score, observations) =>
                observations.Any(o => o.Code == ObservationCodes.LargeRaGuidePulses)
                && observations.Any(o => o.Code == ObservationCodes.LargeDecGuidePulses)
                    ? score + 2
                    : score);

        if (diagnosis != null) {
            yield return diagnosis;
        }
    }
}

