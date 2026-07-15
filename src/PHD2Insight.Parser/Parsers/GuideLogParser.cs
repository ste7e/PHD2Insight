using PHD2Insight.Core.Models;
using PHD2Insight.Parser.Infrastructure;
using PHD2Insight.Parser.Abstractions;

namespace PHD2Insight.Parser.Parsers;

public sealed class GuideLogParser : IGuideLogParser {
    public ParseResult<GuideLog> Parse(Stream stream) {
        ArgumentNullException.ThrowIfNull(stream);

        using var reader = new LineReader(stream);

        while (reader.Read()) {
        }

        return new ParseResult<GuideLog> {
            Success = true,
            Value = new GuideLog()
        };
    }
}