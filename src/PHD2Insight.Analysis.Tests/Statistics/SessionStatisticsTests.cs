using PHD2Insight.Analysis.Models;
using PHD2Insight.Analysis.Statistics;
using PHD2Insight.Core.Models;

namespace PHD2Insight.Analysis.Tests.Statistics;

public sealed class SessionStatisticsTests {
    [Fact]
    public void Calculate_Returns_SessionStatistics() {
        // Arrange
        var session = CreateSession();

        // Act
        SessionStatisticsResult statistics =
            SessionStatistics.Calculate(session);

        // Assert
        Assert.Equal(2, statistics.FrameCount);

        Assert.Equal(
            TimeSpan.FromMinutes(10),
            statistics.Duration);

        Assert.Equal(
            55.0,
            statistics.AverageSignalToNoiseRatio);

        Assert.Equal(
            9000.0,
            statistics.AverageStarMass);
    }

    [Fact]
    public void Calculate_Returns_Zero_Averages_For_Empty_Session() {
        // Arrange
        var session = new GuidingSession {
            StartTime = new DateTime(2026, 1, 1, 20, 0, 0),
            EndTime = new DateTime(2026, 1, 1, 20, 10, 0)
        };

        // Act
        var statistics = SessionStatistics.Calculate(session);

        // Assert
        Assert.Equal(0, statistics.FrameCount);
        Assert.Equal(0.0, statistics.AverageSignalToNoiseRatio);
        Assert.Equal(0.0, statistics.AverageStarMass);
    }

    [Fact]
    public void Calculate_Returns_Null_Duration_For_Open_Session() {
        // Arrange
        var session = new GuidingSession {
            StartTime = new DateTime(2026, 1, 1, 20, 0, 0)
        };

        // Act
        var statistics = SessionStatistics.Calculate(session);

        // Assert
        Assert.Null(statistics.Duration);
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