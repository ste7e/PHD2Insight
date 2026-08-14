using PHD2Insight.Analysis.Diagnostics;
using PHD2Insight.Analysis.Models;
using PHD2Insight.Core.Models;

namespace PHD2Insight.Analysis.Metrics;

public static class GuideCorrectionAnalysis {
    public static GuideCorrectionResult Calculate(
        GuidingSession session) {
        ArgumentNullException.ThrowIfNull(session);

        var frames = AnalysisFrameSelector.GetAnalysisFrames(session);

        var raPulses = frames
            .Where(f => f.RaPulseMilliseconds.HasValue)
            .Select(f => f.RaPulseMilliseconds!.Value)
            .ToList();

        var decPulses = frames
            .Where(f => f.DecPulseMilliseconds.HasValue)
            .Select(f => f.DecPulseMilliseconds!.Value)
            .ToList();

        var raEastCorrectionCount = CountRaCorrections(
            frames,
            GuideDirection.East);

        var raWestCorrectionCount = CountRaCorrections(
            frames,
            GuideDirection.West);

        var decNorthCorrectionCount = CountDecCorrections(
            frames,
            GuideDirection.North);

        var decSouthCorrectionCount = CountDecCorrections(
            frames,
            GuideDirection.South);



        return new GuideCorrectionResult {
            RaCorrectionCount = raPulses.Count,

            AverageRaPulseMilliseconds = raPulses.Count == 0 ? 0 : raPulses.Average(),
            MaximumRaPulseMilliseconds = raPulses.Count == 0 ? 0 : raPulses.Max(),
            TotalRaCorrectionTime = TimeSpan.FromMilliseconds(raPulses.Sum()),

            DecCorrectionCount = decPulses.Count,

            AverageDecPulseMilliseconds = decPulses.Count == 0 ? 0 : decPulses.Average(),
            MaximumDecPulseMilliseconds = decPulses.Count == 0 ? 0 : decPulses.Max(),
            TotalDecCorrectionTime = TimeSpan.FromMilliseconds(decPulses.Sum()),

            RaEastCorrectionCount = raEastCorrectionCount,
            RaWestCorrectionCount = raWestCorrectionCount,

            DecNorthCorrectionCount = decNorthCorrectionCount,
            DecSouthCorrectionCount = decSouthCorrectionCount,

            RaDirectionalImbalance =
                CalculateDirectionalImbalance(
                    raEastCorrectionCount,
                    raWestCorrectionCount),

            DecDirectionalImbalance =
                CalculateDirectionalImbalance(
                    decNorthCorrectionCount,
                    decSouthCorrectionCount)
        };
    }

    private static int CountRaCorrections(IReadOnlyList<GuideFrame> frames, GuideDirection direction) {
        return frames.Count(frame => frame.RaPulseMilliseconds is not null && frame.RaDirection == direction);
    }

    private static int CountDecCorrections(IReadOnlyList<GuideFrame> frames, GuideDirection direction) {
        return frames.Count(frame => frame.DecPulseMilliseconds is not null && frame.DecDirection == direction);
    }

    private static double CalculateDirectionalImbalance(
    int firstDirectionCount,
    int secondDirectionCount) {
        var total = firstDirectionCount + secondDirectionCount;

        if (total == 0) {
            return 0;
        }

        return Math.Abs(firstDirectionCount - secondDirectionCount)
            / (double)total;
    }
}