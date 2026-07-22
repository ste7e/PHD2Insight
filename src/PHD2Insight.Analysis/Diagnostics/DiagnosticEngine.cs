using PHD2Insight.Analysis.Models;

namespace PHD2Insight.Analysis.Diagnostics;

public sealed class DiagnosticEngine {
    private readonly IReadOnlyList<IDiagnosticRule> _rules;

    public DiagnosticEngine(
        IEnumerable<IDiagnosticRule> rules) {
        ArgumentNullException.ThrowIfNull(rules);

        _rules = rules.ToList();
    }

    public IReadOnlyList<Diagnosis> Diagnose(
        AnalysisResult analysis) {
        ArgumentNullException.ThrowIfNull(analysis);

        return _rules
            .SelectMany(rule => rule.Evaluate(analysis))
            .ToList();
    }
}