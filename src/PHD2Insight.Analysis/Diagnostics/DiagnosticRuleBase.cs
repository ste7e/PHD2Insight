using PHD2Insight.Analysis.Models;
using PHD2Insight.Analysis.Observations;

namespace PHD2Insight.Analysis.Diagnostics;

public abstract class DiagnosticRuleBase : IDiagnosticRule {
    public abstract IEnumerable<Diagnosis> Evaluate(
        AnalysisResult analysis);

    protected static Diagnosis? BuildDiagnosis(
        AnalysisResult analysis,
        string code,
        string title,
        string description,
        DiagnosisSeverity severity,
        IReadOnlySet<string> evidenceObservationCodes,
        Func<int, IReadOnlyList<Observation>, int>? contextAdjuster = null) {

        var observations = ObservationEngine.Observe(analysis);

        var supportingObservations = observations
            .Where(o => evidenceObservationCodes.Contains(o.Code))
            .Select(ToSupportingObservation)
            .ToList();

        var score = supportingObservations.Sum(e => e.Weight);

        if (contextAdjuster != null) {
            score = contextAdjuster(score, observations);
        }

        if (score < DiagnosisThresholds.MinimumDiagnosisScore) {
            return null;
        }

        return new Diagnosis {
            Code = code,
            Title = title,
            Description = description,
            Severity = severity,
            Confidence = CalculateConfidence(score),
            SupportingObservations = supportingObservations,
            Score = score,
};
    }

    private static SupportingObservation ToSupportingObservation(
        Observation observation) {

        return new SupportingObservation {
            Code = observation.Code,
            Metric = observation.Metric,
            Value = observation.Value,
            Explanation = observation.Description,
            Weight = observation.Weight
        };
    }

    protected static DiagnosisConfidence CalculateConfidence(
        int score) {

        return score switch {
            >= 8 => DiagnosisConfidence.High,
            >= 6 => DiagnosisConfidence.Medium,
            _ => DiagnosisConfidence.Low
        };
    }
}