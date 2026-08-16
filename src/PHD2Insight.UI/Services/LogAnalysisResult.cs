using PHD2Insight.UI.Services;

namespace PHD2Insight.UI.Services;

public sealed class LogAnalysisResult {

    public string FileName { get; init; } = "";

    public IReadOnlyList<SessionAnalysisResult> Sessions { get; init; }
        = [];
}