using PHD2Insight.Core.Models;
using PHD2Insight.Parser.Abstractions;
using PHD2Insight.Parser.Infrastructure;
using PHD2Insight.Parser.Internal;

namespace PHD2Insight.Parser.Parsers;

public sealed class GuideLogParser : IGuideLogParser {
    public ParseResult<GuideLog> Parse(Stream stream) {
        ArgumentNullException.ThrowIfNull(stream);

        var context = new GuideLogParseContext();

        using var reader = new LineReader(stream);

        while (reader.Read()) {
            var line = reader.CurrentLine!;

            if (VersionLineParser.TryParse(line, out var version)) {
                context.Version = version;
                continue;
            }

            if (SessionStartLineParser.TryParse(line, out var startTime)) {
                context.Sessions.Add(
                    new GuidingSession {
                        StartTime = startTime
                    });

                continue;
            }
        }

        return new ParseResult<GuideLog> {
            Success = true,
            Value = context.Build()
        };
    }
}