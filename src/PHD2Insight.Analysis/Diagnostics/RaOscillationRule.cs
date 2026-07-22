using PHD2Insight.Analysis.Models;

namespace PHD2Insight.Analysis.Diagnostics;

public sealed class RaOscillationRule : IDiagnosticRule {
    public IEnumerable<Diagnosis> Evaluate(AnalysisResult analysis) {
        ArgumentNullException.ThrowIfNull(analysis);

        if (analysis.Rms.RaArcSeconds < DiagnosisThresholds.HighRaRmsArcSeconds)
            yield break;

        if (analysis.OscillationMetrics.RaZeroCrossings <
            DiagnosisThresholds.HighRaZeroCrossings)
            yield break;

        if (analysis.OscillationMetrics.RaDirectionReversals <
            DiagnosisThresholds.HighRaDirectionReversals)
            yield break;

        if (analysis.GuideCorrections.AverageRaPulseMilliseconds <
            DiagnosisThresholds.LargeAverageRaPulseMilliseconds)
            yield break;

        yield return new Diagnosis {
            Code = DiagnosisCodes.RaOscillation,

            Title = "RA Oscillation",

            Description =
                "The RA axis shows evidence of sustained oscillation.",

            Severity = DiagnosisSeverity.Warning,

            Confidence = DiagnosisConfidence.High,

            Evidence =
            [
                new DiagnosisEvidence
                {
                    Metric = "RA RMS",
                    Value = $"{analysis.Rms.RaArcSeconds:F2}\"",
                    Explanation = "RA RMS exceeds the expected range."
                },
                new DiagnosisEvidence
                {
                    Metric = "RA Zero Crossings",
                    Value = analysis.OscillationMetrics.RaZeroCrossings.ToString(),
                    Explanation = "Frequent sign changes indicate oscillatory behaviour."
                },
                new DiagnosisEvidence
                {
                    Metric = "RA Direction Reversals",
                    Value = analysis.OscillationMetrics.RaDirectionReversals.ToString(),
                    Explanation = "The guide error repeatedly changes direction."
                },
                new DiagnosisEvidence
                {
                    Metric = "Average RA Pulse",
                    Value = $"{analysis.GuideCorrections.AverageRaPulseMilliseconds:F0} ms",
                    Explanation = "Guide corrections are consistently large."
                }
            ]
        };
    }
}