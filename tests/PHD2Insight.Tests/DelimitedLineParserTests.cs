using PHD2Insight.Parser.Parsers;

namespace PHD2Insight.Tests;

public sealed class DelimitedLineParserTests {
    [Fact]
    public void Parses_comma_separated_tokens() {
        var tokens = DelimitedLineParser.Parse(
            "A, B, C").ToArray();

        Assert.Equal(
            new[]
            {
                "A",
                "B",
                "C"
            },
            tokens);
    }

    [Fact]
    public void Trims_whitespace() {
        var tokens = DelimitedLineParser.Parse(
            "  A  ,   B   ").ToArray();

        Assert.Equal(
            new[]
            {
                "A",
                "B"
            },
            tokens);
    }

    [Fact]
    public void Ignores_empty_tokens() {
        var tokens = DelimitedLineParser.Parse(
            "A,,B,").ToArray();

        Assert.Equal(
            new[]
            {
                "A",
                "B"
            },
            tokens);
    }

    [Fact]
    public void Supports_custom_delimiter() {
        var tokens = DelimitedLineParser.Parse(
            "A|B|C",
            '|').ToArray();

        Assert.Equal(
            new[]
            {
                "A",
                "B",
                "C"
            },
            tokens);
    }
}