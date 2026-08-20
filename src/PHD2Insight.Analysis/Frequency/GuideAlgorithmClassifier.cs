using PHD2Insight.Parser.Models;

namespace PHD2Insight.Analysis.Frequency;

public static class GuideAlgorithmClassifier {
    public static GuideAlgorithmType Classify(
        GuideAlgorithmInfo? algorithm) {

        if (algorithm is null ||
            string.IsNullOrWhiteSpace(algorithm.Name)) {
            return GuideAlgorithmType.Unknown;
        }

        if (algorithm.Name.Contains(
            "Hysteresis",
            StringComparison.OrdinalIgnoreCase)) {
            return GuideAlgorithmType.Hysteresis;
        }

        if (algorithm.Name.Contains(
            "PEC",
            StringComparison.OrdinalIgnoreCase)) {
            return GuideAlgorithmType.Pec;
        }

        return GuideAlgorithmType.Other;
    }
}