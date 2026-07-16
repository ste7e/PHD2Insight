namespace PHD2Insight.Parser.Parsers;

internal static class EquipmentProfileLineParser {
    public static bool TryParse(
        string line,
        out string? profileName) {
        profileName = null;

        if (!PropertyLineParser.TryParse(line, out var key, out var value)) {
            return false;
        }

        if (!string.Equals(
                key,
                "Equipment Profile",
                StringComparison.Ordinal)) {
            return false;
        }

        profileName = value;

        return true;
    }
}