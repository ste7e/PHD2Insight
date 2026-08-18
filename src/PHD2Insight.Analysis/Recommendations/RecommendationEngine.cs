using PHD2Insight.Analysis.Diagnostics;
using PHD2Insight.Analysis.Models;

namespace PHD2Insight.Analysis.Recommendations;

public sealed class RecommendationEngine {
    private readonly IReadOnlyList<IRecommendationRule> _rules;

    public RecommendationEngine(
        IEnumerable<IRecommendationRule> rules) {
        ArgumentNullException.ThrowIfNull(rules);

        _rules = rules.ToList();
    }

    public IReadOnlyList<Recommendation> Evaluate(
        AnalysisResult analysis,
        IReadOnlyList<Diagnosis> diagnoses) {
        ArgumentNullException.ThrowIfNull(analysis);
        ArgumentNullException.ThrowIfNull(diagnoses);

        return _rules
            .SelectMany(rule => rule.Evaluate(analysis, diagnoses))
            .ToList();
    }
}