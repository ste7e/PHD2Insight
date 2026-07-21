using System.Diagnostics.CodeAnalysis;

namespace PHD2Insight.Analysis.Statistics;

public static class StatisticalFunctions {
    public static double Mean(IEnumerable<double> values) {
        ArgumentNullException.ThrowIfNull(values);

        var data = values.ToArray();

        return data.Length == 0
            ? 0
            : data.Average();
    }

    public static double MeanAbsolute(IEnumerable<double> values) {
        ArgumentNullException.ThrowIfNull(values);

        var data = values
            .Select(System.Math.Abs)
            .ToArray();

        return data.Length == 0
            ? 0
            : data.Average();
    }

    public static double RootMeanSquare(IEnumerable<double> values) {
        ArgumentNullException.ThrowIfNull(values);

        var data = values.ToArray();

        if (data.Length == 0) {
            return 0;
        }

        return System.Math.Sqrt(
            data.Select(v => v * v).Average());
    }

    public static double StandardDeviation(IEnumerable<double> values) {
        ArgumentNullException.ThrowIfNull(values);

        var data = values.ToArray();

        if (data.Length == 0) {
            return 0;
        }

        var mean = data.Average();

        var variance =
            data.Select(v => System.Math.Pow(v - mean, 2))
                .Average();

        return System.Math.Sqrt(variance);
    }

    public static int CountZeroCrossings(IEnumerable<double> values) {
        ArgumentNullException.ThrowIfNull(values);

        var data = values.ToArray();

        if (data.Length < 2) {
            return 0;
        }

        var crossings = 0;

        for (var i = 1; i < data.Length; i++) {
            if ((data[i - 1] < 0 && data[i] > 0) ||
                (data[i - 1] > 0 && data[i] < 0)) {
                crossings++;
            }
        }

        return crossings;
    }

    public static int CountDirectionReversals(IEnumerable<double> values) {
        ArgumentNullException.ThrowIfNull(values);

        var data = values.ToArray();

        if (data.Length < 3) {
            return 0;
        }

        var reversals = 0;

        var previousDelta = data[1] - data[0];

        for (var i = 2; i < data.Length; i++) {
            var currentDelta = data[i] - data[i - 1];

            if ((previousDelta < 0 && currentDelta > 0) ||
                (previousDelta > 0 && currentDelta < 0)) {
                reversals++;
            }

            previousDelta = currentDelta;
        }

        return reversals;
    }
}