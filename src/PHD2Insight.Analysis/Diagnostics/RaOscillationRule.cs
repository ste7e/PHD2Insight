using PHD2Insight.Analysis.Models;

namespace PHD2Insight.Analysis.Diagnostics;

public sealed class RaOscillationRule : IDiagnosticRule {
    public IEnumerable<Diagnosis> Evaluate(AnalysisResult analysis) {
        ArgumentNullException.ThrowIfNull(analysis);

        var evidence = new List<DiagnosisEvidence>();

        AddEvidenceIf(
            analysis.Rms.RaArcSeconds >= DiagnosisThresholds.HighRaRmsArcSeconds,
            evidence,
            EvidenceCodes.HighRaRms,
            "RA RMS",
            $"{analysis.Rms.RaArcSeconds:F2}\"",
            "RA RMS exceeds the expected range.",
            DiagnosisEvidenceWeights.HighRaRms);

        AddEvidenceIf(
            analysis.Rms.RaToDecRatio >= DiagnosisThresholds.HighRaToDecRatio,
            evidence,
            EvidenceCodes.RaDominance,
            "RA/DEC RMS Ratio",
            analysis.Rms.RaToDecRatio.ToString("F2"),
            "RA guiding errors dominate DEC.",
            DiagnosisEvidenceWeights.RaDominance);

        AddEvidenceIf(
            analysis.OscillationMetrics.RaZeroCrossings >=
            DiagnosisThresholds.HighRaZeroCrossings,
            evidence,
            EvidenceCodes.FrequentRaZeroCrossings,
            "RA Zero Crossings",
            analysis.OscillationMetrics.RaZeroCrossings.ToString(),
            "Frequent sign changes indicate oscillatory behaviour.",
            DiagnosisEvidenceWeights.FrequentZeroCrossings);

        AddEvidenceIf(
            analysis.OscillationMetrics.RaDirectionReversals >=
            DiagnosisThresholds.HighRaDirectionReversals,
            evidence,
            EvidenceCodes.FrequentRaDirectionReversals,
            "RA Direction Reversals",
            analysis.OscillationMetrics.RaDirectionReversals.ToString(),
            "Guide corrections repeatedly reverse direction.",
            DiagnosisEvidenceWeights.FrequentDirectionReversals);

        AddEvidenceIf(
            analysis.GuideCorrections.AverageRaPulseMilliseconds >=
            DiagnosisThresholds.LargeAverageRaPulseMilliseconds,
            evidence,
            EvidenceCodes.LargeRaGuidePulses,
            "Average RA Pulse",
            $"{analysis.GuideCorrections.AverageRaPulseMilliseconds:F0} ms",
            "Guide corrections are consistently large.",
            DiagnosisEvidenceWeights.LargeGuidePulses);

        var score = evidence.Sum(e => e.Weight);

        if (score < 4)
            yield break;

        yield return new Diagnosis {
            Code = DiagnosisCodes.RaOscillation,

            Title = "RA Oscillation",

            Description =
                "The RA axis exhibits characteristics consistent with sustained oscillation.",

            Severity = DiagnosisSeverity.Warning,

            Confidence = CalculateConfidence(score),

            Evidence = evidence
        };
    }

    private static void AddEvidenceIf(
        bool condition,
        ICollection<DiagnosisEvidence> evidence,
        string code,
        string metric,
        string value,
        string explanation,
        int weight) {
        if (!condition)
            return;

        evidence.Add(new DiagnosisEvidence {
            Code = code,
            Metric = metric,
            Value = value,
            Explanation = explanation,
            Weight = weight
        });
    }

    private static DiagnosisConfidence CalculateConfidence(int score) {
        return score switch {
            >= 8 => DiagnosisConfidence.High,
            >= 6 => DiagnosisConfidence.Medium,
            _ => DiagnosisConfidence.Low
        };
    }
}