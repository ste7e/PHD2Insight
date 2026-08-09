using PHD2Insight.Analysis.Diagnostics;
using PHD2Insight.Analysis.Models;
using PHD2Insight.Analysis.Observations;

public sealed class DecOscillationDiagnosisRule
    : DiagnosticRuleBase {

    private static readonly HashSet<string> EvidenceObservationCodes = [
        ObservationCodes.HighDecRms,
        ObservationCodes.DecDominance,
        ObservationCodes.MediumDecOscillationRate,
        ObservationCodes.HighDecOscillationRate,
        ObservationCodes.MediumDecOscillationAmplitude,
        ObservationCodes.HighDecOscillationAmplitude,
        ObservationCodes.LargeDecGuidePulses
    ];

    public override IEnumerable<Diagnosis> Evaluate(
        AnalysisResult analysis) {

        var diagnosis = BuildDiagnosis(
            analysis,
            DiagnosisCodes.DecOscillation,
            "DEC Oscillation",
            "The DEC axis exhibits characteristics consistent with sustained oscillation.",
            DiagnosisSeverity.Warning,
            EvidenceObservationCodes);

        if (diagnosis != null) {
            yield return diagnosis;
        }
    }
}