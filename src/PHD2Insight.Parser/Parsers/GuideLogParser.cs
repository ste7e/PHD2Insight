using PHD2Insight.Core.Models;
using PHD2Insight.Parser.Infrastructure;
using PHD2Insight.Parser.Abstractions;

namespace PHD2Insight.Parser.Parsers;

public sealed class GuideLogParser : IGuideLogParser {
    public ParseResult<GuideLog> Parse(Stream stream) {
        ArgumentNullException.ThrowIfNull(stream);

        GuideLogVersion? version = null;

        using var reader = new LineReader(stream);

        while (reader.Read()) {
            if (VersionLineParser.TryParse(
                reader.CurrentLine!,
                out var parsedVersion)) {
                version = parsedVersion;
                break;
            }
        }

        return new ParseResult<GuideLog> {
            Success = version is not null,

            Value = new GuideLog {
                Version = version
            },

            Errors = version is null
                ? new[]
                {
            "No PHD2 version information found."
                }
                : Array.Empty<string>()
        };
    }
}