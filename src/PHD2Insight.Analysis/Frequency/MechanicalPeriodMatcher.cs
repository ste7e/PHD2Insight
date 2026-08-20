using PHD2Insight.Analysis.Frequency;

namespace PHD2Insight.Analysis;

public sealed class MechanicalPeriodMatcher {
    public MechanicalPeriod? Match(
        double measuredPeriodSeconds,
        IReadOnlyList<MechanicalPeriod> periods,
        double tolerancePercent = 2.0) {

        if (!double.IsFinite(measuredPeriodSeconds) ||
            measuredPeriodSeconds <= 0) {
            return null;
        }

        MechanicalPeriod? bestMatch = null;
        double bestError = double.MaxValue;

        foreach (var period in periods) {
            var errorPercent =
                Math.Abs(measuredPeriodSeconds - period.PeriodSeconds)
                / period.PeriodSeconds
                * 100.0;

            if (errorPercent <= tolerancePercent &&
                errorPercent < bestError) {
                bestMatch = period;
                bestError = errorPercent;
            }
        }

        return bestMatch;
    }

    public MechanicalPeriod? MatchWithHarmonic(
        double measuredPeriodSeconds,
        IReadOnlyList<MechanicalPeriod> periods,
        out int? harmonic,
        double tolerancePercent = 2.0,
        int maximumHarmonic = 10) {

        harmonic = null;

        if (!double.IsFinite(measuredPeriodSeconds) ||
            measuredPeriodSeconds <= 0) {
            return null;
        }

        MechanicalPeriod? bestMatch = null;
        var bestError = double.MaxValue;
        var bestHarmonic = 0;

        foreach (var period in periods) {
            for (var h = 1; h <= maximumHarmonic; h++) {
                var expectedPeriod =
                    period.PeriodSeconds / h;

                var errorPercent =
                    Math.Abs(measuredPeriodSeconds - expectedPeriod)
                    / expectedPeriod
                    * 100.0;

                if (errorPercent <= tolerancePercent &&
                    errorPercent < bestError) {
                    bestMatch = period;
                    bestError = errorPercent;
                    bestHarmonic = h;
                }
            }
        }

        if (bestMatch is not null)
            harmonic = bestHarmonic;

        return bestMatch;
    }

    public double CalculateErrorPercent(
        double measuredPeriodSeconds,
        MechanicalPeriod expectedPeriod) {

        if (!double.IsFinite(measuredPeriodSeconds) ||
            measuredPeriodSeconds <= 0) {
            return double.NaN;
        }

        return
            (measuredPeriodSeconds - expectedPeriod.PeriodSeconds)
            / expectedPeriod.PeriodSeconds
            * 100.0;
    }
}