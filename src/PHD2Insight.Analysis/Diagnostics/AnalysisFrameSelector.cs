using PHD2Insight.Core.Models;

namespace PHD2Insight.Analysis.Diagnostics;

internal static class AnalysisFrameSelector {
    public static IReadOnlyList<GuideFrame> GetAnalysisFrames(
        GuidingSession session) {
        if (session.Frames.Count == 0) {
            return Array.Empty<GuideFrame>();
        }

        var excludedFrames = BuildExcludedFrameSet(session);

        return session.Frames
            .Where(f => !excludedFrames.Contains(f.FrameNumber))
            .ToArray();
    }

    private static HashSet<int> BuildExcludedFrameSet(
        GuidingSession session) {
        var excluded = new HashSet<int>();

        TimeSpan? settlingStarted = null;

        foreach (var settlingEvent in session.SettlingEvents) {
            switch (settlingEvent.State) {
                case SettlingState.Started:
                    settlingStarted = settlingEvent.ElapsedTime;
                    break;

                case SettlingState.Completed:

                    if (settlingStarted is not null) {
                        ExcludeFramesBetween(
                            session,
                            settlingStarted.Value,
                            settlingEvent.ElapsedTime,
                            excluded);

                        settlingStarted = null;
                    }

                    break;
            }
        }

        return excluded;
    }

    private static void ExcludeFramesBetween(
    GuidingSession session,
    TimeSpan start,
    TimeSpan end,
    HashSet<int> excluded) {
        foreach (var frame in session.Frames) {
            if (frame.ElapsedTime >= start &&
                frame.ElapsedTime < end) {
                excluded.Add(frame.FrameNumber);
            }
        }
    }

}