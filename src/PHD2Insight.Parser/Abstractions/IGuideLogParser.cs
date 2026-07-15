namespace PHD2Insight.Parser.Abstractions;

using PHD2Insight.Core.Models;

public interface IGuideLogParser {
    ParseResult<GuideLog> Parse(Stream stream);
}