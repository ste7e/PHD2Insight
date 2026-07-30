using PHD2Insight.Analysis.Detection;
using PHD2Insight.Analysis.Tests.Builders;
using Xunit;

namespace PHD2Insight.Analysis.Tests.Detection;

public sealed class OscillationDetectorTests {
    [Fact]
    public void Detect_Returns_No_Events_When_No_Zero_Crossing() {
        var session = new GuidingSessionBuilder()
            .AddFrame(1.0)
            .AddFrame(2.0)
            .AddFrame(3.0)
            .AddFrame(2.0)
            .Build();

        var events = OscillationDetector.Detect(
            session.Frames,
            f => f.RaErrorArcSeconds);

        Assert.Empty(events);
    }

    [Fact]
    public void Detect_Returns_Single_Event_For_One_Complete_Oscillation() {
        var session = new GuidingSessionBuilder()
            .AddFrame(1.0)
            .AddFrame(2.0)
            .AddFrame(3.0)
            .AddFrame(2.0)
            .AddFrame(-1.0)
            .AddFrame(-2.0)
            .AddFrame(-3.0)
            .AddFrame(-2.0)
            .AddFrame(1.0)
            .Build();

        var events = OscillationDetector.Detect(
            session.Frames,
            f => f.RaErrorArcSeconds);

        var oscillation = Assert.Single(events);

        Assert.Equal(3.0, oscillation.PositivePeakArcSeconds);
        Assert.Equal(-3.0, oscillation.NegativePeakArcSeconds);

        Assert.Equal(
            6.0,
            oscillation.PeakToPeakAmplitudeArcSeconds,
            6);
    }

    [Fact]
    public void Detect_Returns_Two_Events_For_Two_Oscillations() {
        var session = new GuidingSessionBuilder()
            .AddFrame(2)
            .AddFrame(3)
            .AddFrame(-2)
            .AddFrame(-3)
            .AddFrame(2)
            .AddFrame(3)
            .AddFrame(-2)
            .AddFrame(-3)
            .AddFrame(2)
            .Build();

        var events = OscillationDetector.Detect(
            session.Frames,
            f => f.RaErrorArcSeconds);

        Assert.Collection(
            events,

            e => {
                Assert.Equal(3, e.PositivePeakArcSeconds);
                Assert.Equal(-3, e.NegativePeakArcSeconds);
            },

            e => {
                Assert.Equal(3, e.PositivePeakArcSeconds);
                Assert.Equal(-3, e.NegativePeakArcSeconds);
            },

            e => {
                Assert.Equal(3, e.PositivePeakArcSeconds);
                Assert.Equal(-3, e.NegativePeakArcSeconds);
            });
    }

    [Fact]
    public void Detect_Ignores_Incomplete_Oscillation_At_End_Of_Log() {
        var session = new GuidingSessionBuilder()
            .AddFrame(1)
            .AddFrame(2)
            .AddFrame(3)
            .AddFrame(-1)
            .AddFrame(-2)
            .AddFrame(-3)
            .Build();

        var events = OscillationDetector.Detect(
            session.Frames,
            f => f.RaErrorArcSeconds);

        Assert.Empty(events);
    }

    [Fact]
    public void Detect_Ignores_Oscillation_Below_Deadband() {
        var session = new GuidingSessionBuilder()
            .AddFrame(1, TimeSpan.Zero, 0.10)
            .AddFrame(2, TimeSpan.FromSeconds(2), 0.15)
            .AddFrame(3, TimeSpan.FromSeconds(4), 0.10)
            .AddFrame(4, TimeSpan.FromSeconds(6), -0.10)
            .AddFrame(5, TimeSpan.FromSeconds(8), -0.15)
            .AddFrame(6, TimeSpan.FromSeconds(10), -0.10)
            .AddFrame(7, TimeSpan.FromSeconds(12), 0.10)
            .Build();

        var events = OscillationDetector.Detect(
            session.Frames,
            f => f.RaErrorArcSeconds);

        Assert.Empty(events);
    }

    [Fact]
    public void Detect_Returns_Oscillation_Above_Deadband() {
        var session = new GuidingSessionBuilder()
            .AddFrame(1, TimeSpan.Zero, 0.30)
            .AddFrame(2, TimeSpan.FromSeconds(2), 0.45)
            .AddFrame(3, TimeSpan.FromSeconds(4), 0.30)
            .AddFrame(4, TimeSpan.FromSeconds(6), -0.30)
            .AddFrame(5, TimeSpan.FromSeconds(8), -0.45)
            .AddFrame(6, TimeSpan.FromSeconds(10), -0.30)
            .AddFrame(7, TimeSpan.FromSeconds(12), 0.30)
            .Build();

        var events = OscillationDetector.Detect(
            session.Frames,
            f => f.RaErrorArcSeconds);

        Assert.Single(events);
    }
}