using PHD2Insight.Parser.Parsers;

namespace PHD2Insight.Tests.Parsers;

public sealed class KeyValueSequenceParserTests {
    [Fact]
    public void Parses_multiple_key_value_pairs() {
        var result = KeyValueSequenceParser.Parse(
            "Minimum move = 0.300 Aggression = 70% FastSwitch = disabled")
            .ToList();

        Assert.Collection(
            result,

            pair => {
                Assert.Equal("Minimum move", pair.Key);
                Assert.Equal("0.300", pair.Value);
            },

            pair => {
                Assert.Equal("Aggression", pair.Key);
                Assert.Equal("70%", pair.Value);
            },

            pair => {
                Assert.Equal("FastSwitch", pair.Key);
                Assert.Equal("disabled", pair.Value);
            });
    }

    [Fact]
    public void Empty_string_returns_no_pairs() {
        Assert.Empty(
            KeyValueSequenceParser.Parse(string.Empty));
    }

    [Fact]
    public void Ignores_missing_value() {
        var result = KeyValueSequenceParser.Parse(
            "Aggression =")
            .ToList();

        Assert.Empty(result);
    }

    [Fact]
    public void Ignores_missing_key() {
        var result = KeyValueSequenceParser.Parse(
            "= 0.300")
            .ToList();

        Assert.Empty(result);
    }
}