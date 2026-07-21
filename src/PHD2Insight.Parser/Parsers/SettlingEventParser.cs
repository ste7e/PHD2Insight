namespace PHD2Insight.Parser.Parsers;

using PHD2Insight.Core.Models;
using System.Diagnostics.CodeAnalysis;

internal static class SettlingEventParser {
    private const string Prefix =
        "INFO: SETTLING STATE CHANGE,";

    internal static bool TryParse(
        string line,
        [NotNullWhen(true)] out SettlingEvent? settlingEvent) {
        ArgumentNullException.ThrowIfNull(line);

        const string Prefix = "INFO: SETTLING STATE CHANGE,";

        if (!line.StartsWith(Prefix, StringComparison.Ordinal)) {
            settlingEvent = null;
            return false;
        }

        settlingEvent = new SettlingEvent {
            State = SettlingStateParser.Parse(line[Prefix.Length..].Trim())
        };

        return true;
    }
}