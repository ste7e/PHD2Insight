using System.Globalization;

namespace PHD2Insight.Parser.Parsers;

internal static class InvariantNumberParser {
    public static bool TryParseDouble(
        string? value,
        out double result) {
        return double.TryParse(
            value,
            NumberStyles.Float,
            CultureInfo.InvariantCulture,
            out result);
    }
}