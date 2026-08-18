using PHD2Insight.Analysis.Diagnostics;
using PHD2Insight.Analysis.Models;

namespace PHD2Insight.Analysis.Recommendations;

public interface IRecommendationRule {
    IEnumerable<Recommendation> Evaluate(
        AnalysisResult analysis,
        IReadOnlyList<Diagnosis> diagnoses);
}