using PHD2Insight.Analysis.Models;
using PHD2Insight.Core.Models;

namespace PHD2Insight.Analysis.Statistics;

public static class SessionStatistics {
    public static SessionStatisticsResult Calculate(
        GuidingSession session) {
        ArgumentNullException.ThrowIfNull(session);

        return new SessionStatisticsResult {
            FrameCount = session.Frames.Count,

            Duration = session.EndTime is null
                ? null
                : session.EndTime.Value - session.StartTime,

            AverageSignalToNoiseRatio =
                session.Frames.Count == 0
                    ? 0
                    : session.Frames.Average(f => f.SignalToNoiseRatio),

            AverageStarMass =
                session.Frames.Count == 0
                    ? 0
                    : session.Frames.Average(f => f.StarMass)
        };
    }


}