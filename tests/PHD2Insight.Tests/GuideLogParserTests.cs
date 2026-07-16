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

        var first = result.Value!.Sessions[0];

        Assert.NotNull(first.Camera);

        Assert.Equal(
            "QHY5II-M-10d7d910f33c7354",
            first.Camera.Name);
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

    [Fact]
    public void Parses_camera_info_line() {
        const string line =
            "Camera = QHY5II-M-10d7d910f33c7354, gain = 90, full size = 1280 x 1024, have dark, dark dur = 3000, no defect map, pixel size = 5.2 um";
        var ok = CameraInfoLineParser.TryParse(
            line,
            out var result);
        Assert.True(ok);
        Assert.NotNull(result);

        Assert.Equal(
            "QHY5II-M-10d7d910f33c7354",
            result.Name);

        Assert.Equal(90, result.Gain);

        Assert.Equal(5.2, result.PixelSizeMicrons);
    }

    [Fact]
    public void Parses_camera_line_with_leading_whitespace() {
        const string line =
            " Camera = QHY5II-M-10d7d910f33c7354, gain = 90, pixel size = 5.2 um";

        var result = CameraInfoLineParser.TryParse(
            line,
            out var camera);

        Assert.True(result);
        Assert.NotNull(camera);

        Assert.Equal(
            "QHY5II-M-10d7d910f33c7354",
            camera.Name);
    }
}