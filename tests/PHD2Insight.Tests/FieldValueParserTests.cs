using PHD2Insight.Parser.Parsers;

namespace PHD2Insight.Tests.Parsers;

public sealed class FieldValueParserTests {
    private static readonly IReadOnlyList<string> Fields =
    [
        "123",
        "4.56",
        "Mount",
        ""
    ];

    [Fact]
    public void Reads_string() {
        Assert.True(
            FieldValueParser.TryGetString(
                Fields,
                2,
                out var value));

        Assert.Equal("Mount", value);
    }

    [Fact]
    public void Reads_int32() {
        Assert.True(
            FieldValueParser.TryGetInt32(
                Fields,
                0,
                out var value));

        Assert.Equal(123, value);
    }

    [Fact]
    public void Reads_double() {
        Assert.True(
            FieldValueParser.TryGetDouble(
                Fields,
                1,
                out var value));

        Assert.Equal(4.56, value);
    }

    [Fact]
    public void Returns_false_for_missing_field() {
        Assert.False(
            FieldValueParser.TryGetString(
                Fields,
                10,
                out _));
    }

    [Fact]
    public void Returns_false_for_invalid_double() {
        Assert.False(
            FieldValueParser.TryGetDouble(
                Fields,
                2,
                out _));
    }

    [Fact]
    public void Returns_false_for_empty_field() {
        Assert.False(
            FieldValueParser.TryGetDouble(
                Fields,
                3,
                out _));
    }
}