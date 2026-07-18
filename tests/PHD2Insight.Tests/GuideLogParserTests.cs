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
    public void Parser_of_full_sample() {
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

        Assert.NotNull(first.Mount);

        Assert.Equal(
            "EQMOD HEQ5/6 (ASCOM)",
            first.Mount!.Name);

        Assert.Equal(
            -81.6,
            first.Mount.XAngleDegrees);

        Assert.Equal(
            "Predictive PEC",
            first.xGuideAlgorithm!.Name);

        Assert.Equal(
            "Resist Switch",
            first.yGuideAlgorithm!.Name);

        Assert.NotNull(first.xGuideAlgorithm);

        Assert.Equal(
            "Predictive PEC",
            first.xGuideAlgorithm!.Name);

        Assert.Equal(
            0.600,
            first.xGuideAlgorithm.ControlGain);

        Assert.Equal(
            0.350,
            first.xGuideAlgorithm.PredictionGain);

        var schema = first.GuideFrameSchema;
        Assert.NotNull(schema);

        Assert.Equal(18, schema.Columns.Count);

        Assert.Equal("Frame", schema.Columns.ElementAt(0));
        Assert.Equal("Time", schema.Columns.ElementAt(1));
        Assert.Equal("mount", schema.Columns.ElementAt(2));
        Assert.Equal("ErrorCode", schema.Columns.Last());

        Assert.NotEmpty(first.Frames);

        var firstFrame = first.Frames.First();

        Assert.Equal(1, firstFrame.FrameNumber);
        Assert.Equal(TimeSpan.FromSeconds(9.161), firstFrame.ElapsedTime);

        Assert.Equal(9223.0, firstFrame.StarMass);
        Assert.Equal(59.51, firstFrame.SignalToNoiseRatio);
        Assert.Equal(0, firstFrame.ErrorCode);
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

    [Fact]
    public void Parses_mount_metadata() {
        const string line =
            "Mount = EQMOD HEQ5/6 (ASCOM), connected, guiding enabled, xAngle = -81.6, xRate = 4.604, yAngle = 17.1, yRate = 6.929, parity = +/-";

        var result = MountInfoLineParser.TryParse(
            line,
            out var mount);

        Assert.True(result);

        Assert.NotNull(mount);

        Assert.Equal(
            "EQMOD HEQ5/6 (ASCOM)",
            mount.Name);

        Assert.True(mount.Connected);

        Assert.True(mount.GuidingEnabled);

        Assert.Equal(-81.6, mount.XAngleDegrees);

        Assert.Equal(4.604, mount.XRate);

        Assert.Equal(17.1, mount.YAngleDegrees);

        Assert.Equal(6.929, mount.YRate);

        Assert.Equal(
            "+/-",
            mount.Parity);
    }
}