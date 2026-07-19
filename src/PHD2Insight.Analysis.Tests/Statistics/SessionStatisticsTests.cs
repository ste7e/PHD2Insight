using PHD2Insight.Analysis.Statistics;
using PHD2Insight.Core.Models;

namespace PHD2Insight.Analysis.Tests.Statistics;

public class SessionStatisticsTests {
    [Fact]
    public void FrameCount_Returns_NumberOfFrames() {
        var session = CreateSession();

        Assert.Equal(2, SessionStatistics.FrameCount(session));
    }

    [Fact]
    public void Duration_Returns_SessionDuration() {
        var session = CreateSession();

        Assert.Equal(
            TimeSpan.FromMinutes(10),
            SessionStatistics.Duration(session));
    }

    [Fact]
    public void AverageSignalToNoiseRatio_Returns_MeanValue() {
        var session = CreateSession();

        Assert.Equal(
            55.0,
            SessionStatistics.AverageSignalToNoiseRatio(session));
    }

    [Fact]
    public void AverageStarMass_Returns_MeanValue() {
        var session = CreateSession();

        Assert.Equal(
            9000.0,
            SessionStatistics.AverageStarMass(session));
    }

    private static GuidingSession CreateSession() {
        return new GuidingSession {
            StartTime = new DateTime(2026, 1, 1, 20, 0, 0),
            EndTime = new DateTime(2026, 1, 1, 20, 10, 0),

            Frames =
            [
                new GuideFrame
                {
                    SignalToNoiseRatio = 50,
                    StarMass = 8000
                },

                new GuideFrame
                {
                    SignalToNoiseRatio = 60,
                    StarMass = 10000
                }
            ]
        };
    }
}