using PHD2Insight.Analysis.Diagnostics;
using PHD2Insight.Analysis.Models;
using PHD2Insight.Analysis.Observations;

public sealed class AggressiveGuidingDiagnosisRule
    : DiagnosticRuleBase {

    private static readonly HashSet<string> EvidenceObservationCodes = [
        ObservationCodes.HighRaOscillationRate,
        ObservationCodes.HighDecOscillationRate,

        ObservationCodes.MediumRaOscillationAmplitude,
        ObservationCodes.MediumDecOscillationAmplitude,

        ObservationCodes.HighRaOscillationAmplitude,
        ObservationCodes.HighDecOscillationAmplitude,

        ObservationCodes.LargeRaGuidePulses,
        ObservationCodes.LargeDecGuidePulses
    ];

    public override IEnumerable<Diagnosis> Evaluate(
        AnalysisResult analysis) {

        var diagnosis = BuildDiagnosis(
            analysis,
            DiagnosisCodes.AggressiveGuiding,
            "Guide Aggressiveness Too High",
            "Rapid guiding oscillations are consistent with overly aggressive guide corrections.",
            DiagnosisSeverity.Warning,
            EvidenceObservationCodes);

        if (diagnosis != null) {
            yield return diagnosis;
        }
    }

}