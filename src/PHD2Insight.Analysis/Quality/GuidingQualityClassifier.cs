using PHD2Insight.Analysis.Models;

namespace PHD2Insight.Analysis.Quality;

public sealed class GuidingQualityClassifier {

    public double GoodMaximumTotalRms { get; init; } = 1.0;

    public double AcceptableMaximumTotalRms { get; init; } = 2.0;


    public GuidingQuality Classify(
        double? totalRms) {

        if (!totalRms.HasValue ||
            double.IsNaN(totalRms.Value) ||
            double.IsInfinity(totalRms.Value)) {

            return GuidingQuality.Unknown;
        }

        if (totalRms.Value < GoodMaximumTotalRms)
            return GuidingQuality.Good;

        if (totalRms.Value < AcceptableMaximumTotalRms)
            return GuidingQuality.Acceptable;

        return GuidingQuality.Poor;
    }
}