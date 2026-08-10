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

        var observations = ObservationEngine.Observe(analysis);

        var hasHighRaRate =
            observations.Any(o =>
                o.Code == ObservationCodes.HighRaOscillationRate);

        var hasHighDecRate =
            observations.Any(o =>
                o.Code == ObservationCodes.HighDecOscillationRate);

        var hasRaAmplitude =
            observations.Any(o =>
                o.Code == ObservationCodes.MediumRaOscillationAmplitude ||
                o.Code == ObservationCodes.HighRaOscillationAmplitude);

        var hasDecAmplitude =
            observations.Any(o =>
                o.Code == ObservationCodes.MediumDecOscillationAmplitude ||
                o.Code == ObservationCodes.HighDecOscillationAmplitude);

        var hasCoherentEvidence =
            (hasHighRaRate && hasRaAmplitude) ||
            (hasHighDecRate && hasDecAmplitude) ||
            (hasHighRaRate && hasHighDecRate);

        if (!hasCoherentEvidence) {
            yield break;
        }

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
