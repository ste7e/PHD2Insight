using PHD2Insight.Analysis.Detection;
using PHD2Insight.Analysis.Diagnostics;
using PHD2Insight.Analysis.Frequency;
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

        var periodProfile = MountPeriodProfiles.Eq6RPro;
        var periodMatcher = new MechanicalPeriodMatcher();

        var raFrequency = AnalyseFrequency(
            frames,
            s => s.RaGuideDistance);

        var decFrequency = AnalyseFrequency(
            frames,
            s => s.DecGuideDistance);

        var raMechanicalPeriods = AnalyseMechanicalPeriods(
            frames,
            s => s.RaGuideDistance,
            MountPeriodProfiles.Eq6RPro.RaPeriods);

        var decMechanicalPeriods = AnalyseMechanicalPeriods(
            frames,
            s => s.DecGuideDistance,
            MountPeriodProfiles.Eq6RPro.DecPeriods);

        var raErrors = frames
            .Select(f => f.RaErrorArcSeconds)
            .ToArray();

        var decErrors = frames
            .Select(f => f.DecErrorArcSeconds)
            .ToArray();

        int raDirectionReversals = StatisticalFunctions.CountDirectionReversals(raErrors);
        int decDirectionReversals = StatisticalFunctions.CountDirectionReversals(decErrors);

        var raEvents = OscillationDetector.Detect(frames, f => f.RaErrorArcSeconds);

        var decEvents = OscillationDetector.Detect(frames, f => f.DecErrorArcSeconds);

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

            MeanDecOscillationAmplitudeArcSeconds = MeanAmplitude(decEvents),

            RaDominantFrequencyHz = raFrequency?.FrequencyHz,
            RaDominantPeriodSeconds = raFrequency?.PeriodSeconds,
            RaFrequencyPower = raFrequency?.Power,

            DecDominantFrequencyHz = decFrequency?.FrequencyHz,
            DecDominantPeriodSeconds = decFrequency?.PeriodSeconds,
            DecFrequencyPower = decFrequency?.Power,

            RaMechanicalPeriods = raMechanicalPeriods,

            DecMechanicalPeriods = decMechanicalPeriods,

            MechanicalPeriodPower = AnalyseRaMechanicalPeriodPower(frames),

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

    private static LombScargleResult? AnalyseFrequency(
    IReadOnlyList<GuideFrame> frames,
    Func<GuideFrame, double> valueSelector) {
        if (frames.Count < 10)
            return null;

        var times = frames
            .Select(s => s.ElapsedTime.TotalSeconds)
            .ToArray();

        var values = frames
            .Select(valueSelector)
            .ToArray();

        if (times.Length < 10)
            return null;

        var medianInterval = CalculateMedianInterval(times);

        // The shortest useful period is approximately twice the
        // typical sampling interval. We deliberately use the actual
        // median interval rather than assuming a fixed cadence.
        var minimumPeriod = Math.Max(
            medianInterval * 2.0,
            2.0);

        // Don't try to identify a period longer than half the session.
        // There aren't enough cycles to make such a result meaningful.
        var span = times[^1] - times[0];

        var maximumPeriod = span / 2.0;

        if (maximumPeriod <= minimumPeriod)
            return null;

        return LombScarglePeriodogram.FindDominantFrequency(
            times,
            values,
            minimumPeriod,
            maximumPeriod);
    }

    private static IReadOnlyList<MechanicalPeriodAnalysisResult>
    AnalyseMechanicalPeriods(
        IReadOnlyList<GuideFrame> frames,
        Func<GuideFrame, double> valueSelector,
        IReadOnlyList<MechanicalPeriod> periods) {
        if (frames.Count < 10 || periods.Count == 0) {
            return Array.Empty<MechanicalPeriodAnalysisResult>();
        }

        var times = frames
            .Select(f => f.ElapsedTime.TotalSeconds)
            .ToArray();

        var values = frames
            .Select(valueSelector)
            .ToArray();

        if (times.Length < 10) {
            return Array.Empty<MechanicalPeriodAnalysisResult>();
        }

        var results = new List<MechanicalPeriodAnalysisResult>();

        foreach (var period in periods) {
            if (!double.IsFinite(period.PeriodSeconds) ||
                period.PeriodSeconds <= 0) {
                continue;
            }

            var frequencyHz = 1.0 / period.PeriodSeconds;

            var result =
                LombScarglePeriodogram.EvaluateFrequency(
                    times,
                    values,
                    frequencyHz);

            if (result is null) {
                continue;
            }

            results.Add(
                new MechanicalPeriodAnalysisResult(
                    period,
                    period.PeriodSeconds,
                    frequencyHz,
                    result.Power));
        }

        return results;
    }

    private static MechanicalPeriodPowerResult AnalyseRaMechanicalPeriodPower(
    IReadOnlyList<GuideFrame> frames) {
        if (frames.Count < 10) {
            return new MechanicalPeriodPowerResult {
                IsValid = false
            };
        }

        var wormPeriod = MountPeriodProfiles.Eq6RPro.RaPeriods
            .FirstOrDefault(p =>
                p.Name.Equals(
                    "RA worm fundamental",
                    StringComparison.OrdinalIgnoreCase));

        if (wormPeriod is null) {
            return new MechanicalPeriodPowerResult {
                IsValid = false
            };
        }

        var times = frames
            .Select(f => f.ElapsedTime.TotalSeconds)
            .ToArray();

        var values = frames
            .Select(f => f.RaGuideDistance)
            .ToArray();

        var frequencyHz = 1.0 / wormPeriod.PeriodSeconds;

        var result = LombScarglePeriodogram.EvaluateFrequency(
            times,
            values,
            frequencyHz);

        if (result is null || !double.IsFinite(result.Power)) {
            return new MechanicalPeriodPowerResult {
                IsValid = false
            };
        }

        var amplitude =
            LombScarglePeriodogram.EvaluateAmplitude(
                times,
                values,
                frequencyHz);

        if (amplitude is null ||
            !double.IsFinite(amplitude.Value)) {
            return new MechanicalPeriodPowerResult {
                IsValid = false
            };
        }
        return new MechanicalPeriodPowerResult {
            RaWormFundamentalArcSeconds = amplitude.Value,
            RaWormFundamentalPower = result.Power,
            RaWormPeriodSeconds = wormPeriod.PeriodSeconds,
            IsValid = true
        };

    }

    private static double CalculateMedianInterval(
        IReadOnlyList<double> times) {
        if (times.Count < 2)
            return double.NaN;

        var intervals = new double[times.Count - 1];

        for (var i = 1; i < times.Count; i++) {
            intervals[i - 1] = times[i] - times[i - 1];
        }

        Array.Sort(intervals);

        var middle = intervals.Length / 2;

        return intervals.Length % 2 == 0
            ? (intervals[middle - 1] + intervals[middle]) / 2.0
            : intervals[middle];
    }
}
