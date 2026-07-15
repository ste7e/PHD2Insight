using System.Globalization;

namespace PHD2Insight.Parser.Parsers;

internal static class SessionStartLineParser {
    private const string Prefix = "Guiding Begins at ";

    public static bool TryParse(
        string line,
        out DateTime startTime) {
        startTime = default;

        if (!line.StartsWith(Prefix))
            return false;

        var value = line[Prefix.Length..];

        return DateTime.TryParse(
            value,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out startTime);
    }
}