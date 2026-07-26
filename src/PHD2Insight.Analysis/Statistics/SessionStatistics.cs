using PHD2Insight.Analysis.Diagnostics;
using PHD2Insight.Analysis.Models;
using PHD2Insight.Core.Models;

namespace PHD2Insight.Analysis.Statistics;

public static class SessionStatistics {
    public static SessionStatisticsResult Calculate(
        GuidingSession session) {
        ArgumentNullException.ThrowIfNull(session);

        var frames = AnalysisFrameSelector.GetAnalysisFrames(session);

        return new SessionStatisticsResult {
            FrameCount = frames.Count,

            Duration = session.EndTime is null
                ? null
                : session.EndTime.Value - session.StartTime,

            AverageSignalToNoiseRatio =
                frames.Count == 0
                    ? 0
                    : frames.Average(f => f.SignalToNoiseRatio),

            AverageStarMass =
                frames.Count == 0
                    ? 0
                    : frames.Average(f => f.StarMass)
        };
    }


}