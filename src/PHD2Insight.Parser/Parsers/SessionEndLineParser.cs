using System.Globalization;

namespace PHD2Insight.Parser.Parsers;

internal static class SessionEndLineParser {
    private const string Prefix = "Guiding Ends at ";

    public static bool TryParse(
        string line,
        out DateTime endTime) {
        endTime = default;

        if (!line.StartsWith(Prefix))
            return false;

        var value = line[Prefix.Length..];

        return DateTime.TryParse(
            value,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out endTime);
    }
}