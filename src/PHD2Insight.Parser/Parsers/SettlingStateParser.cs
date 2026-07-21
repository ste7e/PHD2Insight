namespace PHD2Insight.Parser.Parsers;

using PHD2Insight.Core.Models;

internal static class SettlingStateParser {
    internal static SettlingState Parse(string value) {
        ArgumentNullException.ThrowIfNull(value);

        return value.Trim() switch {
            "Settling started" => SettlingState.Started,

            "Settling complete" => SettlingState.Completed,

            "Settling failed" => SettlingState.Failed,

            "Settling cancelled" => SettlingState.Cancelled,

            _ => SettlingState.Unknown
        };
    }
}