namespace PHD2Insight.Analysis.Frequency;

public sealed record FrequencyAnalysisResult {
    public FrequencyPeak? Ra { get; init; }

    public FrequencyPeak? Dec { get; init; }
}
