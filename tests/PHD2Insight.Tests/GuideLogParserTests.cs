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
    public void Parser_extracts_guiding_session_start_and_end_times() {
        IGuideLogParser parser = new GuideLogParser();

        using var stream = File.OpenRead(TestData.SampleGuideLog);

        var result = parser.Parse(stream);

        Assert.True(result.Success);
        Assert.NotNull(result.Value);

        Assert.Equal(4, result.Value.Sessions.Count);

        var first = result.Value.Sessions[0];

        Assert.Equal(
            new DateTime(2026, 7, 9, 22, 54, 58),
            first.StartTime);

        Assert.Equal(
            new DateTime(2026, 7, 9, 22, 58, 34),
            first.EndTime);

        Assert.Equal(3000, first.ExposureMilliseconds);
    }

    [Fact]
    public void Parses_pixel_scale_line() {
        const string line =
            "Pixel scale = 1.05 arc-sec/px, Binning = 1, Focal length = 1018 mm";

        var ok = PixelScaleLineParser.TryParse(
            line,
            out var result);

        Assert.True(ok);

        Assert.NotNull(result);

        Assert.Equal(1.05, result!.PixelScale);

        Assert.Equal(1, result.Binning);

        Assert.Equal(1018, result.FocalLengthMm);
    }
}