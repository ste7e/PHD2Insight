using System.Diagnostics.CodeAnalysis;

namespace PHD2Insight.Analysis.Statistics;

internal static class StatisticalFunctions {
    public static double RootMeanSquare(
        IEnumerable<double> values) {
        ArgumentNullException.ThrowIfNull(values);

        double sum = 0;
        int count = 0;

        foreach (double value in values) {
            sum += value * value;
            count++;
        }

        return count == 0
            ? 0
            : Math.Sqrt(sum / count);
    }
}