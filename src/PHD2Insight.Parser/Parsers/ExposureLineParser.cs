using System.Globalization;

namespace PHD2Insight.Parser.Parsers;

internal static class ExposureLineParser {
    public static bool TryParse(
        string line,
        out int exposureMilliseconds) {
        exposureMilliseconds = 0;

        if (!PropertyLineParser.TryParse(
                line,
                out var key,
                out var value)) {
            return false;
        }

        if (!string.Equals(
                key,
                "Exposure",
                StringComparison.Ordinal)) {
            return false;
        }

        const string suffix = " ms";

        if (!value.EndsWith(
                suffix,
                StringComparison.Ordinal)) {
            return false;
        }

        var number = value[..^suffix.Length];

        return int.TryParse(
            number,
            NumberStyles.None,
            CultureInfo.InvariantCulture,
            out exposureMilliseconds);
    }
}