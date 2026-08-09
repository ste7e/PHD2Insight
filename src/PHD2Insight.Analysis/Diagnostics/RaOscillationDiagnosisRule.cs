using PHD2Insight.Analysis.Diagnostics;
using PHD2Insight.Analysis.Models;
using PHD2Insight.Analysis.Observations;

public sealed class RaOscillationDiagnosisRule
    : DiagnosticRuleBase {

    private static readonly HashSet<string> EvidenceObservationCodes = [
        ObservationCodes.HighRaRms,
        ObservationCodes.RaDominance,
        ObservationCodes.MediumRaOscillationRate,
        ObservationCodes.HighRaOscillationRate,
        ObservationCodes.MediumRaOscillationAmplitude,
        ObservationCodes.HighRaOscillationAmplitude,
        ObservationCodes.LargeRaGuidePulses
    ];

    public override IEnumerable<Diagnosis> Evaluate(
        AnalysisResult analysis) {

        var diagnosis = BuildDiagnosis(
            analysis,
            DiagnosisCodes.RaOscillation,
            "RA Oscillation",
            "The RA axis exhibits characteristics consistent with sustained oscillation.",
            DiagnosisSeverity.Warning,
            EvidenceObservationCodes,
            AdjustForContext);

        if (diagnosis != null) {
            yield return diagnosis;
        }
    }

    private static int AdjustForContext(
        int score,
        IReadOnlyList<Observation> observations) {

        if (observations.Any(o =>
            o.Code == ObservationCodes.SevereLostStars)) {

            score = Math.Max(0, score - 2);
        }

        return score;
    }
}