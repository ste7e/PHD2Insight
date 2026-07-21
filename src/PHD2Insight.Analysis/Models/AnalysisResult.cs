using PHD2Insight.Analysis.Statistics;

namespace PHD2Insight.Analysis.Models;

public sealed record AnalysisResult {
    public SessionStatisticsResult SessionStatistics { get; init; } = new();

    public RmsResult Rms { get; init; } = new();

    public GuideCorrectionResult GuideCorrections { get; init; } = new();

    public PeakErrorResult PeakErrors { get; init; } = new();

    public LostStarResult LostStars { get; init; } = new();
}