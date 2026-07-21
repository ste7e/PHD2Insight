using PHD2Insight.Analysis.Models;
using PHD2Insight.Analysis.Statistics;
using PHD2Insight.Core.Models;

namespace PHD2Insight.Analysis.Metrics;

public static class OscillationMetricsAnalysis {
    public static OscillationMetricsResult Calculate(GuidingSession session) {
        ArgumentNullException.ThrowIfNull(session);

        var raErrors = session.Frames
            .Select(f => f.RaErrorArcSeconds)
            .ToArray();

        var decErrors = session.Frames
            .Select(f => f.DecErrorArcSeconds)
            .ToArray();

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

            RaZeroCrossings =
                StatisticalFunctions.CountZeroCrossings(raErrors),

            DecZeroCrossings =
                StatisticalFunctions.CountZeroCrossings(decErrors),

            RaDirectionReversals =
                StatisticalFunctions.CountDirectionReversals(raErrors),

            DecDirectionReversals =
                StatisticalFunctions.CountDirectionReversals(decErrors)
        };
    }
}