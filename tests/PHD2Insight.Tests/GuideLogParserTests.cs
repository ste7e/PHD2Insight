using PHD2Insight.Parser.Abstractions;
using PHD2Insight.Parser.Parsers;
using Xunit;

namespace PHD2Insight.Tests;

public sealed class GuideLogParserTests {
    [Fact]
    public void Parser_returns_success_for_sample_log() {
        // Arrange
        IGuideLogParser parser = new GuideLogParser();

        using var stream = File.OpenRead(TestData.SampleGuideLog);

        // Act
        var result = parser.Parse(stream);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Parser_extracts_phd2_version() {
        // Arrange
        IGuideLogParser parser = new GuideLogParser();

        using var stream = File.OpenRead(TestData.SampleGuideLog);

        // Act
        var result = parser.Parse(stream);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.NotNull(result.Value.Version);

        Assert.Equal(
            "2.6.14 [Windows]",
            result.Value.Version.Phd2Version);

        Assert.Equal(
            "2.5",
            result.Value.Version.LogVersion);
    }
}