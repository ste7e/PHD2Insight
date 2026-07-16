namespace PHD2Insight.Parser.Parsers;

internal static class DelimitedLineParser {
    public static IEnumerable<string> Parse(
        string line,
        char delimiter = ',') {
        ArgumentNullException.ThrowIfNull(line);

        var start = 0;

        for (var i = 0; i <= line.Length; i++) {
            if (i == line.Length || line[i] == delimiter) {
                var token = line[start..i].Trim();

                if (token.Length > 0) {
                    yield return token;
                }

                start = i + 1;
            }
        }
    }
}