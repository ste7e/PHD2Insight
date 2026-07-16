namespace PHD2Insight.Parser.Parsers;

internal static class KeyValueSequenceParser {
    public static IEnumerable<(string Key, string Value)> Parse(
        string text) {
        ArgumentNullException.ThrowIfNull(text);

        var tokens = text.Split(
            ' ',
            StringSplitOptions.RemoveEmptyEntries);

        var keyParts = new List<string>(4);

        for (var i = 0; i < tokens.Length; i++) {
            var token = tokens[i];

            if (token != "=") {
                keyParts.Add(token);
                continue;
            }

            // '=' with no preceding key.
            if (keyParts.Count == 0) {
                continue;
            }

            // '=' with no following value.
            if (i + 1 >= tokens.Length) {
                yield break;
            }

            yield return (
                string.Join(" ", keyParts),
                tokens[i + 1]);

            keyParts.Clear();

            // Skip over the value we just consumed.
            i++;
        }
    }
}