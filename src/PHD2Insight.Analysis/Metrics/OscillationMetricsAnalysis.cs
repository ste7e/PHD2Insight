using PHD2Insight.Analysis.Detection;
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

        int raDirectionReversals = StatisticalFunctions.CountDirectionReversals(raErrors);
        int decDirectionReversals = StatisticalFunctions.CountDirectionReversals(decErrors);

        var raEvents = OscillationDetector.Detect( frames, f => f.RaErrorArcSeconds);

        var decEvents = OscillationDetector.Detect( frames, f => f.DecErrorArcSeconds);

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

            RaOscillationEventsPerMinute = CalculateRatePerMinute(raEvents.Count, frames),

            DecOscillationEventsPerMinute = CalculateRatePerMinute(decEvents.Count, frames),

            RaDirectionReversals = raDirectionReversals,

            RaDirectionChangesPerMinute = CalculateRatePerMinute(raDirectionReversals, frames),

            DecDirectionReversals = decDirectionReversals,

            DecDirectionChangesPerMinute = CalculateRatePerMinute(decDirectionReversals, frames),

            MeanRaOscillationAmplitudeArcSeconds = MeanAmplitude(raEvents),

            MeanDecOscillationAmplitudeArcSeconds = MeanAmplitude(decEvents)

        };
    }
    private static bool IsSignificantCrossing(
    double previous,
    double current) {
        var amplitude = Math.Abs(previous - current);

        if (amplitude < OscillationThresholds.MinimumOscillationAmplitudeArcSeconds) {
            return false;
        }

        if (System.Math.Abs(previous)
            < OscillationThresholds.MinimumOscillationAmplitudeArcSeconds) {
            return false;
        }

        if (System.Math.Abs(current)
            < OscillationThresholds.MinimumOscillationAmplitudeArcSeconds) {
            return false;
        }

        return System.Math.Sign(previous)
            != System.Math.Sign(current);
    }

    private static double CalculateRatePerMinute(
    int count,
    IReadOnlyList<GuideFrame> frames) {
        if (frames.Count < 2) {
            return 0;
        }

        var duration = frames[^1].ElapsedTime - frames[0].ElapsedTime;

        return duration.TotalMinutes <= 0
            ? 0
            : count / duration.TotalMinutes;
    }

    private static double CalculateRatePerMinute(
            IReadOnlyList<OscillationEvent> events,
            IReadOnlyList<GuideFrame> frames) {
        if (events.Count == 0 || frames.Count < 2) {
            return 0;
        }

        var duration =
            frames[^1].ElapsedTime - frames[0].ElapsedTime;

        if (duration.TotalMinutes <= 0) {
            return 0;
        }

        return events.Count / duration.TotalMinutes;
    }

    private static double MeanAmplitude(
        IReadOnlyList<OscillationEvent> events) {
        return events.Count == 0
            ? 0
            : events.Average(e => e.MeanAmplitudeArcSeconds);
    }
}
