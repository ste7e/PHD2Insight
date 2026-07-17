using System.Text;

namespace PHD2Insight.Parser.Parsers;

internal static class CsvLineParser {
    public static IReadOnlyList<string> Parse(string line) {
        ArgumentNullException.ThrowIfNull(line);

        var fields = new List<string>();
        var builder = new StringBuilder();

        var inQuotes = false;

        foreach (var ch in line) {
            switch (ch) {
                case '"':
                    inQuotes = !inQuotes;
                    break;

                case ',' when !inQuotes:
                    fields.Add(builder.ToString());
                    builder.Clear();
                    break;

                default:
                    builder.Append(ch);
                    break;
            }
        }

        fields.Add(builder.ToString());

        return fields;
    }
}