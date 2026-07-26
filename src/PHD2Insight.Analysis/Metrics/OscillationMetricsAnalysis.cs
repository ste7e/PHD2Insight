using PHD2Insight.Analysis.Diagnostics;
using PHD2Insight.Analysis.Models;
using PHD2Insight.Analysis.Statistics;
using PHD2Insight.Core.Models;

namespace PHD2Insight.Analysis.Metrics;

public static class OscillationMetricsAnalysis {
    public static OscillationMetricsResult Calculate(GuidingSession session) {
        ArgumentNullException.ThrowIfNull(session);

        var frames = AnalysisFrameSelector.GetAnalysisFrames(session);

        if (frames.Count < 2) {
            return new OscillationMetricsResult();
        }

        var duration =
            frames[^1].ElapsedTime - frames[0].ElapsedTime;

        if (duration <= TimeSpan.Zero) {
            return new OscillationMetricsResult();
        }

        var raErrors = frames
            .Select(f => f.RaErrorArcSeconds)
            .ToArray();

        var decErrors = frames
            .Select(f => f.DecErrorArcSeconds)
            .ToArray();

        double minutes = (frames.Last().ElapsedTime - frames.First().ElapsedTime) .TotalMinutes;

        int raZeroCrossings = StatisticalFunctions.CountZeroCrossings(raErrors);
        int decZeroCrossings = StatisticalFunctions.CountZeroCrossings(decErrors);

        int raDirectionReversals = StatisticalFunctions.CountDirectionReversals(raErrors);
        int decDirectionReversals = StatisticalFunctions.CountDirectionReversals(decErrors);

        return new OscillationMetricsResult {
            MeanRaErrorArcSeconds =
                StatisticalFunctions.Mean(raErrors),

            MeanDecErrorArcSeconds =
                StatisticalFunctions.Mean(decErrors),

            MeanAbsoluteRaErrorArcSeconds =
                StatisticalFunctions.MeanAbsolute(raErrors),

            MeanAbsoluteDecErrorArcSeconds =
                StatisticalFunctions.MeanAbsolute(decErrors),

            StandardDeviationRaErrorArcSeconds =
                StatisticalFunctions.StandardDeviation(raErrors),

            StandardDeviationDecErrorArcSeconds =
                StatisticalFunctions.StandardDeviation(decErrors),

            RaZeroCrossings = raZeroCrossings,

            RaZeroCrossingsPerMinute = raZeroCrossings / minutes,

            DecZeroCrossings = decZeroCrossings,

            DecZeroCrossingsPerMinute = decZeroCrossings / minutes,

            RaDirectionReversals = raDirectionReversals,

            RaDirectionChangesPerMinute = raDirectionReversals / minutes,

            DecDirectionReversals = decDirectionReversals,

            DecDirectionChangesPerMinute = decDirectionReversals / minutes,



        };
    }
}