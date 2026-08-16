using PHD2Insight.Analysis.Diagnostics;
using PHD2Insight.Analysis.Models;
using PHD2Insight.Core.Models;

namespace PHD2Insight.UI.Services;

public sealed record SessionAnalysisResult(
    int SessionNumber,
    GuidingSession Session,
    AnalysisResult Analysis,
    IReadOnlyList<Diagnosis> Diagnoses,
    GuidingQuality Quality);