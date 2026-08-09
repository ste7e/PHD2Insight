using PHD2Insight.Analysis.Diagnostics;
using PHD2Insight.Analysis.Models;
using PHD2Insight.Core.Models;

namespace PHD2Insight.Analysis.Metrics;

public static class GuideReversalAnalysis {
    public static GuideReversalResult Calculate(
        GuidingSession session) {

        ArgumentNullException.ThrowIfNull(session);

        var frames = AnalysisFrameSelector.GetAnalysisFrames(session);

        if (frames.Count < 2) {
            return new GuideReversalResult();
        }

        var durationMinutes =
            (frames[^1].ElapsedTime - frames[0].ElapsedTime)
            .TotalMinutes;

        if (durationMinutes <= 0) {
            return new GuideReversalResult();
        }

        var raReversals = CountRaGuideReversals(frames);
        var decReversals = CountDecGuideReversals(frames);

        return new GuideReversalResult {
            RaReversalCount = raReversals,
            DecReversalCount = decReversals,

            RaReversalRatePerMinute =
                raReversals / durationMinutes,

            DecReversalRatePerMinute =
                decReversals / durationMinutes
        };
    }
    private static int CountRaGuideReversals(
    IReadOnlyList<GuideFrame> frames) {

        var count = 0;

        GuideFrame? previous = null;

        foreach (var frame in frames) {

            if (frame.RaPulseMilliseconds is null ||
                frame.RaDirection == GuideDirection.None) {
                continue;
            }

            if (previous is not null &&
                previous.RaPulseMilliseconds is not null &&
                previous.RaDirection != GuideDirection.None &&
                previous.RaDirection != frame.RaDirection &&
                previous.RaErrorArcSeconds * frame.RaErrorArcSeconds < 0) {

                count++;
            }

            previous = frame;
        }

        return count;
    }

    private static int CountDecGuideReversals(
    IReadOnlyList<GuideFrame> frames) {

        var count = 0;

        GuideFrame? previous = null;

        foreach (var frame in frames) {

            if (frame.DecPulseMilliseconds is null ||
                frame.DecDirection == GuideDirection.None) {
                continue;
            }

            if (previous is not null &&
                previous.DecPulseMilliseconds is not null &&
                previous.DecDirection != GuideDirection.None &&
                previous.DecDirection != frame.DecDirection &&
                previous.DecErrorArcSeconds * frame.DecErrorArcSeconds < 0) {

                count++;
            }

            previous = frame;
        }

        return count;
    }
}