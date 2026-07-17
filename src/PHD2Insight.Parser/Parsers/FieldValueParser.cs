using System.Diagnostics.CodeAnalysis;

namespace PHD2Insight.Parser.Parsers;

internal static class FieldValueParser {
    public static bool TryGetString(
        IReadOnlyList<string> fields,
        int index,
        [NotNullWhen(true)] out string? value) {
        value = null;

        if (index < 0 || index >= fields.Count) {
            return false;
        }

        value = fields[index];

        return true;
    }

    public static bool TryGetInt32(
        IReadOnlyList<string> fields,
        int index,
        out int value) {
        value = default;

        return
            TryGetString(fields, index, out var text) &&
            int.TryParse(text, out value);
    }

    public static bool TryGetDouble(
        IReadOnlyList<string> fields,
        int index,
        out double value) {
        value = default;

        return
            TryGetString(fields, index, out var text) &&
            InvariantNumberParser.TryParseDouble(
                text,
                out value);
    }
    public static bool TryGetEnum<TEnum>(
        IReadOnlyList<string> fields,
        int index,
        out TEnum value)
        where TEnum : struct, Enum {
        value = default;

        return
            TryGetString(fields, index, out var text) &&
            Enum.TryParse(text, true, out value);
    }
}