using PHD2Insight.Core.Models;

namespace PHD2Insight.Analysis.Statistics;

public static class SessionStatistics {
    public static int FrameCount(GuidingSession session) {
        ArgumentNullException.ThrowIfNull(session);

        return session.Frames.Count;
    }

    public static TimeSpan? Duration(GuidingSession session) {
        ArgumentNullException.ThrowIfNull(session);

        return session.EndTime is null
            ? null
            : session.EndTime.Value - session.StartTime;
    }

    public static double AverageSignalToNoiseRatio(GuidingSession session) {
        ArgumentNullException.ThrowIfNull(session);

        return session.Frames.Count == 0
            ? 0
            : session.Frames.Average(f => f.SignalToNoiseRatio);
    }

    public static double AverageStarMass(GuidingSession session) {
        ArgumentNullException.ThrowIfNull(session);

        return session.Frames.Count == 0
            ? 0
            : session.Frames.Average(f => f.StarMass);
    }
}