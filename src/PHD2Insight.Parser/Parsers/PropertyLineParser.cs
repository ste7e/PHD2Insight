using System.Diagnostics.CodeAnalysis;

namespace PHD2Insight.Parser.Parsers;

internal static class PropertyLineParser {
    public static bool TryParse(
        string line,
        [NotNullWhen(true)] out string? key,
        [NotNullWhen(true)] out string? value) {
        key = null;
        value = null;

        ArgumentNullException.ThrowIfNull(line);

        var separator = line.IndexOf('=');

        if (separator < 0) {
            return false;
        }

        key = line[..separator].Trim();
        value = line[(separator + 1)..].Trim();

        return key.Length > 0;
    }
}