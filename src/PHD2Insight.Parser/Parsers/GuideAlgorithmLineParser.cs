using System.Diagnostics.CodeAnalysis;
using PHD2Insight.Core.Models;

namespace PHD2Insight.Parser.Parsers;

internal static class GuideAlgorithmLineParser {
    public static bool TryParse(
        string line,
        out GuideAxis axis,
        [NotNullWhen(true)] out GuideAlgorithmInfo? algorithm) {
        axis = default;
        algorithm = null;

        ArgumentNullException.ThrowIfNull(line);

        line = line.TrimStart();

        var comma = line.IndexOf(',');

        string left;
        string? right = null;

        if (comma >= 0) {
            left = line[..comma];
            right = line[(comma + 1)..].TrimStart();
        } else {
            left = line;
        }

        if (!PropertyLineParser.TryParse(
                left,
                out var key,
                out var value)) {
            return false;
        }

        switch (key) {
            case "X guide algorithm":
                axis = GuideAxis.X;
                break;

            case "Y guide algorithm":
                axis = GuideAxis.Y;
                break;

            default:
                return false;
        }

        var info = new GuideAlgorithmInfo {
            Name = value!
        };

        if (right is not null) {
            foreach (var (property, propertyValue)
                     in KeyValueSequenceParser.Parse(right)) {
                switch (property) {
                    case "Control gain":
                        if (InvariantNumberParser.TryParseDouble(
                                propertyValue,
                                out var controlGain)) {
                            info.ControlGain = controlGain;
                        }
                        break;

                    case "Minimum move":
                        if (InvariantNumberParser.TryParseDouble(
                                propertyValue,
                                out var minimumMove)) {
                            info.MinimumMove = minimumMove;
                        }
                        break;

                    case "Aggression": {
                            var text = propertyValue;

                            if (text.EndsWith("%", StringComparison.Ordinal)) {
                                text = text[..^1];
                            }

                            if (InvariantNumberParser.TryParseDouble(
                                    text,
                                    out var aggression)) {
                                info.Aggression = aggression;
                            }

                            break;
                        }

                    case "FastSwitch":
                        info.FastSwitch =
                                string.Equals(
                                    propertyValue,
                                    "enabled",
                                    StringComparison.OrdinalIgnoreCase);
                        break;

                    case "Prediction gain":
                        if (InvariantNumberParser.TryParseDouble(
                                propertyValue,
                                out var predictionGain)) {
                            info.PredictionGain = predictionGain;
                        }
                        break;
                }
            }
        }

        algorithm = info;
        return true;
    }
}