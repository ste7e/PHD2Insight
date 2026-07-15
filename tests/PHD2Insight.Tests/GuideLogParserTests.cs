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

    [Fact]
    public void Parser_extracts_guiding_sessions() {
        IGuideLogParser parser = new GuideLogParser();

        using var stream = File.OpenRead(TestData.SampleGuideLog);

        var result = parser.Parse(stream);

        Assert.True(result.Success);
        Assert.NotNull(result.Value);

        Assert.Equal(4, result.Value.Sessions.Count);

        Assert.Equal(
            new DateTime(2026, 7, 9, 22, 54, 58),
            result.Value.Sessions[0].StartTime);
    }

}