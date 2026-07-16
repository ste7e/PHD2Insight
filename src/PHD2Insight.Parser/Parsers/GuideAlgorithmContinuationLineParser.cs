using PHD2Insight.Core.Models;

namespace PHD2Insight.Parser.Parsers;

internal static class GuideAlgorithmContinuationLineParser {
    public static bool TryApply(
        string line,
        GuideAlgorithmInfo algorithm) {
        ArgumentNullException.ThrowIfNull(line);
        ArgumentNullException.ThrowIfNull(algorithm);

        if (!PropertyLineParser.TryParse(
                line,
                out var key,
                out var value)) {
            return false;
        }

        switch (key) {
            case "Prediction gain":
                if (InvariantNumberParser.TryParseDouble(
                        value,
                        out var predictionGain)) {
                    algorithm.PredictionGain = predictionGain;
                    return true;
                }
                break;

            case "Minimum move":
                if (InvariantNumberParser.TryParseDouble(
                        value,
                        out var minimumMove)) {
                    algorithm.MinimumMove = minimumMove;
                    return true;
                }
                break;

            case "Control gain":
                if (InvariantNumberParser.TryParseDouble(
                        value,
                        out var controlGain)) {
                    algorithm.ControlGain = controlGain;
                    return true;
                }
                break;

            case "Aggression": {
                    var text = value!;

                    if (text.EndsWith("%", StringComparison.Ordinal)) {
                        text = text[..^1];
                    }

                    if (InvariantNumberParser.TryParseDouble(
                            text,
                            out var aggression)) {
                        algorithm.Aggression = aggression;
                        return true;
                    }

                    break;
                }

            case "FastSwitch":
                algorithm.FastSwitch =
                    string.Equals(
                        value,
                        "enabled",
                        StringComparison.OrdinalIgnoreCase);

                return true;
        }

        return false;
    }
}