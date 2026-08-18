using PHD2Insight.Analysis.Diagnostics;
using PHD2Insight.Analysis.Models;

namespace PHD2Insight.Analysis.Recommendations;

public sealed class ReviewRaGuidingAggressivenessRule : IRecommendationRule {
    public IEnumerable<Recommendation> Evaluate(
        AnalysisResult analysis,
        IReadOnlyList<Diagnosis> diagnoses) {
        ArgumentNullException.ThrowIfNull(analysis);
        ArgumentNullException.ThrowIfNull(diagnoses);

        var diagnosis = diagnoses.FirstOrDefault(
            d => d.Code == "RA_OSCILLATION");

        if (diagnosis is null)
            yield break;

        yield return new Recommendation {
            Code = "REVIEW_RA_GUIDING_AGGRESSIVENESS",
            Title = "Review RA guiding aggressiveness",
            Description =
                "RA guiding shows signs of oscillation. " +
                "Consider reducing RA aggressiveness or reviewing " +
                "the mount's response to guide corrections.",
            Priority = RecommendationPriority.Medium,
            SupportingDiagnosisCodes = new[]
            {
                diagnosis.Code
            },
            SupportingObservationCodes = diagnosis
                .SupportingObservations
                .Select(o => o.Code)
                .ToArray()
        };
    }
}