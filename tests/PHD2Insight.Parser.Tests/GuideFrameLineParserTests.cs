using PHD2Insight.Parser.Parsers;

namespace PHD2Insight.Tests.Parsers;

public sealed class GuideFrameLineParserTests {
    
    [Fact]
    public void Parses_Pulse_Durations() {
        const string Line =
            "1,9.161,\"Mount\",1.189,-0.946,1.109,1.195,0.665,0.000,145,W,0,,,,9223,59.51,0";

        var result =
            GuideFrameLineParser.TryParse(
                Line,
                out var frame);

        Assert.True(result);

        Assert.NotNull(frame);

        Assert.Equal(1, frame.FrameNumber);

        Assert.Equal(
            TimeSpan.FromSeconds(9.161),
            frame.ElapsedTime);

        Assert.Equal(1.189, frame.RaErrorPixels);
        Assert.Equal(-0.946, frame.DecErrorPixels);

        Assert.Equal(0.665, frame.RaGuideDistance);
        Assert.Equal(0.000, frame.DecGuideDistance);

        Assert.Equal(145.0, frame.RaPulseMilliseconds);
        Assert.Null(frame.DecPulseMilliseconds);

        Assert.Equal(GuideDirection.West, frame.RaDirection);
        Assert.Equal(GuideDirection.None, frame.DecDirection);

        Assert.Equal(9223.0, frame.StarMass);
        Assert.Equal(59.51, frame.SignalToNoiseRatio);
        Assert.Equal(0, frame.ErrorCode);
    }

    [Fact]
    public void Rejects_drop_frame() {
        const string Line =
            "710,5341.627,\"DROP\",,,,,,,,,,,,,0,0.00,3,\"Star lost - low mass\"";

        Assert.False(
            GuideFrameLineParser.TryParse(
                Line,
                out _));
    }

    [Fact]
    public void Rejects_header() {
        Assert.False(
            GuideFrameLineParser.TryParse(
                "Frame,Time,mount,dx,dy,RARawDistance,DECRawDistance,RAGuideDistance,DECGuideDistance,RADuration,RADirection,DECDuration,DECDirection,XStep,YStep,StarMass,SNR,ErrorCode",
                out _));
    }
}