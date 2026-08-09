using PHD2Insight.Core.Models;
using PHD2Insight.Parser.Internal;
using PHD2Insight.Parser.Models;
using System.Diagnostics.CodeAnalysis;

namespace PHD2Insight.Parser.Parsers;

internal static class LostStarLineParser {

    private static class Columns {
        public const int Time = 1;
        public const int EventType = 2;
        public const int ErrorCode = 17;
        public const int ErrorMessage = 18;
    }

    public static bool TryParse(
        string line,
        [NotNullWhen(true)] out LostStarEvent? lostStarEvent) {

        lostStarEvent = null;

        var fields = CsvLineParser.Parse(line);

        if (!FieldValueParser.TryGetString(
                fields,
                Columns.EventType,
                out var eventType)) {
            return false;
        }

        if (!string.Equals(
                eventType,
                "DROP",
                StringComparison.Ordinal)) {
            return false;
        }

        if (!FieldValueParser.TryGetDouble(
                fields,
                Columns.Time,
                out var elapsedSeconds)) {
            return false;
        }

        if (!FieldValueParser.TryGetInt32(
                fields,
                Columns.ErrorCode,
                out var errorCode)) {
            return false;
        }

        if (!FieldValueParser.TryGetString(
                fields,
                Columns.ErrorMessage,
                out var errorMessage)) {
            return false;
        }

        lostStarEvent = new LostStarEvent {
            ElapsedTime = TimeSpan.FromSeconds(elapsedSeconds),
            ErrorCode = errorCode,
            ErrorMessage = errorMessage
        };

        return true;
    }
}