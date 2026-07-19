using PHD2Insight.Parser.Parsers;
using Xunit;

namespace PHD2Insight.Tests;

public sealed class ExposureLineParserTests {
    [Fact]
    public void Parses_exposure() {
        var result = ExposureLineParser.TryParse(
            "Exposure = 3000 ms",
            out var exposure);

        Assert.True(result);
        Assert.Equal(3000, exposure);
    }

    [Fact]
    public void Rejects_invalid_units() {
        var result = ExposureLineParser.TryParse(
            "Exposure = 3000",
            out _);

        Assert.False(result);
    }

    [Fact]
    public void Rejects_non_exposure_line() {
        var result = ExposureLineParser.TryParse(
            "Camera = QHY5II-M",
            out _);

        Assert.False(result);
    }
}