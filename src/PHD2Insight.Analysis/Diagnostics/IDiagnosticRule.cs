using PHD2Insight.Analysis.Models;

namespace PHD2Insight.Analysis.Diagnostics;

public interface IDiagnosticRule {
    IEnumerable<Diagnosis> Evaluate(
        AnalysisResult analysis);
}