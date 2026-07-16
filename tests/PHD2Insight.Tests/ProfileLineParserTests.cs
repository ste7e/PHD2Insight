using PHD2Insight.Parser.Parsers;

public sealed class PropertyLineParserTests {
    [Theory]
    [InlineData("Exposure = 3000 ms", "Exposure", "3000 ms")]
    [InlineData("Camera = QHY5II-M", "Camera", "QHY5II-M")]
    [InlineData("A=B", "A", "B")]
    public void Parses_property_lines(
        string line,
        string expectedKey,
        string expectedValue) {
        var result = PropertyLineParser.TryParse(
            line,
            out var key,
            out var value);

        Assert.True(result);
        Assert.Equal(expectedKey, key);
        Assert.Equal(expectedValue, value);
    }

    [Fact]
    public void Rejects_lines_without_separator() {
        var result = PropertyLineParser.TryParse(
            "Guiding Begins at 2026-07-09 22:54:58",
            out _,
            out _);

        Assert.False(result);
    }
}