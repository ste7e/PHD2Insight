namespace PHD2Insight.Parser.Parsers;

internal static class EquipmentProfileLineParser {
    private const string Prefix = "Equipment Profile = ";

    public static bool TryParse(
        string line,
        out string? profileName) {
        profileName = null;

        if (!line.StartsWith(Prefix, StringComparison.Ordinal)) {
            return false;
        }

        profileName = line[Prefix.Length..].Trim();

        return profileName.Length > 0;
    }
}