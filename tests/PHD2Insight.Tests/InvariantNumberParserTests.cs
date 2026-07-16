using PHD2Insight.Parser.Parsers;

namespace PHD2Insight.Tests;

public sealed class InvariantNumberParserTests {
    [Fact]
    public void Parses_decimal_number() {
        var result = InvariantNumberParser.TryParseDouble(
            "4.604",
            out var value);

        Assert.True(result);
        Assert.Equal(4.604, value);
    }


    [Fact]
    public void Parses_negative_number() {
        var result = InvariantNumberParser.TryParseDouble(
            "-81.6",
            out var value);

        Assert.True(result);
        Assert.Equal(-81.6, value);
    }


    [Fact]
    public void Rejects_invalid_number() {
        var result = InvariantNumberParser.TryParseDouble(
            "not a number",
            out _);

        Assert.False(result);
    }
}