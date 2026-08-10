using PHD2Insight.Analysis.Diagnostics;
using PHD2Insight.Analysis.Models;
using PHD2Insight.Analysis.Observations;

public sealed class PoorTransparencyDiagnosisRule
    : DiagnosticRuleBase {

    private static readonly HashSet<string> EvidenceObservationCodes = [
        ObservationCodes.FrequentLostStars,
        ObservationCodes.SevereLostStars,

        ObservationCodes.LowRaOscillationAmplitude,
        ObservationCodes.LowDecOscillationAmplitude,

        ObservationCodes.NormalRaGuidePulses,
        ObservationCodes.NormalDecGuidePulses
    ];

    public override IEnumerable<Diagnosis> Evaluate(
        AnalysisResult analysis) {

        var observations = ObservationEngine.Observe(analysis);

        var hasLostStarEvidence =
            observations.Any(o =>
                o.Code == ObservationCodes.FrequentLostStars ||
                o.Code == ObservationCodes.SevereLostStars);

        if (!hasLostStarEvidence) {
            yield break;
        }

        var diagnosis = BuildDiagnosis(
            analysis,
            DiagnosisCodes.PoorTransparency,
            "Poor Transparency",
            "Guide star loss is consistent with cloud, haze or poor atmospheric transparency.",
            DiagnosisSeverity.Warning,
            EvidenceObservationCodes);

        if (diagnosis != null) {
            yield return diagnosis;
        }
    }
}