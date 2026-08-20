namespace PHD2Insight.Analysis.Frequency;

public sealed record LombScargleResult(
    double FrequencyHz,
    double PeriodSeconds,
    double Power);

public static class LombScarglePeriodogram {
    /// <summary>
    /// Finds the strongest periodic component in an irregularly sampled
    /// time series using the normalized Lomb-Scargle periodogram.
    /// </summary>
    /// <param name="times">Sample times in seconds.</param>
    /// <param name="values">Sample values corresponding to <paramref name="times"/>.</param>
    /// <param name="minimumPeriodSeconds">
    /// Shortest period to search.
    /// </param>
    /// <param name="maximumPeriodSeconds">
    /// Longest period to search.
    /// </param>
    /// <param name="frequencySteps">
    /// Number of frequencies evaluated across the search range.
    /// </param>
    public static LombScargleResult? FindDominantFrequency(
        IReadOnlyList<double> times,
        IReadOnlyList<double> values,
        double minimumPeriodSeconds,
        double maximumPeriodSeconds,
        int frequencySteps = 1000) {
        if (times.Count != values.Count)
            throw new ArgumentException("Times and values must contain the same number of samples.");

        if (times.Count < 10)
            return null;

        if (minimumPeriodSeconds <= 0)
            throw new ArgumentOutOfRangeException(nameof(minimumPeriodSeconds));

        if (maximumPeriodSeconds <= minimumPeriodSeconds)
            throw new ArgumentException(
                "Maximum period must be greater than minimum period.");

        if (frequencySteps < 2)
            throw new ArgumentOutOfRangeException(nameof(frequencySteps));

        var samples = times
            .Zip(values)
            .Where(x =>
                double.IsFinite(x.First) &&
                double.IsFinite(x.Second))
            .OrderBy(x => x.First)
            .ToArray();

        if (samples.Length < 10)
            return null;

        var t = samples.Select(x => x.First).ToArray();
        var y = samples.Select(x => x.Second).ToArray();

        var mean = y.Average();

        var variance = y
            .Select(v => (v - mean) * (v - mean))
            .Average();

        if (variance <= double.Epsilon)
            return null;

        var minimumFrequency = 1.0 / maximumPeriodSeconds;
        var maximumFrequency = 1.0 / minimumPeriodSeconds;

        LombScargleResult? best = null;

        for (var i = 0; i < frequencySteps; i++) {
            var fraction = (double)i / (frequencySteps - 1);

            var frequency =
                minimumFrequency +
                fraction * (maximumFrequency - minimumFrequency);

            var power = CalculatePower(
                t,
                y,
                mean,
                variance,
                frequency);

            if (!double.IsFinite(power))
                continue;

            var period = 1.0 / frequency;

            if (best is null || power > best.Power) {
                best = new LombScargleResult(
                    frequency,
                    period,
                    power);
            }
        }

        return best;
    }

    /// <summary>
    /// Evaluates the normalized Lomb-Scargle power at one specific frequency.
    /// </summary>
    /// <param name="times">Sample times in seconds.</param>
    /// <param name="values">Sample values corresponding to <paramref name="times"/>.</param>
    /// <param name="frequencyHz">Frequency to evaluate in Hz.</param>
    public static LombScargleResult? EvaluateFrequency(
        IReadOnlyList<double> times,
        IReadOnlyList<double> values,
        double frequencyHz) {
        if (times.Count != values.Count)
            throw new ArgumentException(
                "Times and values must contain the same number of samples.");

        if (times.Count < 10)
            return null;

        if (!double.IsFinite(frequencyHz) || frequencyHz <= 0)
            throw new ArgumentOutOfRangeException(nameof(frequencyHz));

        var samples = times
            .Zip(values)
            .Where(x =>
                double.IsFinite(x.First) &&
                double.IsFinite(x.Second))
            .OrderBy(x => x.First)
            .ToArray();

        if (samples.Length < 10)
            return null;

        var t = samples.Select(x => x.First).ToArray();
        var y = samples.Select(x => x.Second).ToArray();

        var mean = y.Average();

        var variance = y
            .Select(v => (v - mean) * (v - mean))
            .Average();

        if (variance <= double.Epsilon)
            return null;

        var power = CalculatePower(
            t,
            y,
            mean,
            variance,
            frequencyHz);

        if (!double.IsFinite(power))
            return null;

        return new LombScargleResult(
            frequencyHz,
            1.0 / frequencyHz,
            power);
    }

    private static double CalculatePower(
        double[] times,
        double[] values,
        double mean,
        double variance,
        double frequency) {
        var omega = 2.0 * Math.PI * frequency;

        double sin2Sum = 0;
        double cos2Sum = 0;
        double sinCosSum = 0;

        for (var i = 0; i < times.Length; i++) {
            var angle = omega * times[i];

            var sin = Math.Sin(angle);
            var cos = Math.Cos(angle);

            sin2Sum += sin * sin;
            cos2Sum += cos * cos;
            sinCosSum += sin * cos;
        }

        var tauNumerator = 2.0 * sinCosSum;
        var tauDenominator = cos2Sum - sin2Sum;

        var tau =
            Math.Atan2(tauNumerator, tauDenominator)
            / (2.0 * omega);

        double shiftedSin2Sum = 0;
        double shiftedCos2Sum = 0;

        double shiftedSinWeightedSum = 0;
        double shiftedCosWeightedSum = 0;

        for (var i = 0; i < times.Length; i++) {
            var angle = omega * (times[i] - tau);

            var sin = Math.Sin(angle);
            var cos = Math.Cos(angle);

            shiftedSin2Sum += sin * sin;
            shiftedCos2Sum += cos * cos;

            var value = values[i] - mean;

            shiftedSinWeightedSum += value * sin;
            shiftedCosWeightedSum += value * cos;
        }

        if (shiftedSin2Sum <= double.Epsilon ||
            shiftedCos2Sum <= double.Epsilon) {
            return double.NaN;
        }

        return
            0.5 / variance *
            (
                (shiftedCosWeightedSum * shiftedCosWeightedSum)
                    / shiftedCos2Sum
                +
                (shiftedSinWeightedSum * shiftedSinWeightedSum)
                    / shiftedSin2Sum
            );
    }
}