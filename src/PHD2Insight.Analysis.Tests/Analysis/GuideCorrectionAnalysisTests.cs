using PHD2Insight.Analysis.Metrics;
using PHD2Insight.Core.Models;

namespace PHD2Insight.Analysis.Tests.Metrics;

public sealed class GuideCorrectionAnalysisTests {
    [Fact]
    public void Calculate_Returns_GuideCorrectionStatistics() {
        // Arrange
        var session = CreateSession();

        // Act
        var result = GuideCorrectionAnalysis.Calculate(session);

        // Assert
        Assert.Equal(3, result.RaCorrectionCount);
        Assert.Equal(2, result.DecCorrectionCount);

        Assert.Equal(200.0, result.AverageRaPulseMilliseconds);
        Assert.Equal(75.0, result.AverageDecPulseMilliseconds);

        Assert.Equal(300, result.MaximumRaPulseMilliseconds);
        Assert.Equal(100, result.MaximumDecPulseMilliseconds);

        Assert.Equal(
            TimeSpan.FromMilliseconds(600),
            result.TotalRaCorrectionTime);

        Assert.Equal(
            TimeSpan.FromMilliseconds(150),
            result.TotalDecCorrectionTime);
    }

    [Fact]
    public void Calculate_Ignores_Missing_Pulses() {
        // Arrange
        var session = new GuidingSession {
            Frames =
            [
                new GuideFrame
                {
                    RaPulseMilliseconds = null
                },
                new GuideFrame
                {
                    RaPulseMilliseconds = 100
                },
                new GuideFrame
                {
                    RaPulseMilliseconds = null
                },
                new GuideFrame
                {
                    RaPulseMilliseconds = 300
                }
            ]
        };

        // Act
        var result = GuideCorrectionAnalysis.Calculate(session);

        // Assert
        Assert.Equal(2, result.RaCorrectionCount);
        Assert.Equal(200.0, result.AverageRaPulseMilliseconds);
        Assert.Equal(300, result.MaximumRaPulseMilliseconds);

        Assert.Equal(
            TimeSpan.FromMilliseconds(400),
            result.TotalRaCorrectionTime);
    }

    [Fact]
    public void Calculate_Returns_Zero_For_Empty_Session() {
        // Arrange
        var session = new GuidingSession();

        // Act
        var result = GuideCorrectionAnalysis.Calculate(session);

        // Assert
        Assert.Equal(0, result.RaCorrectionCount);
        Assert.Equal(0, result.DecCorrectionCount);

        Assert.Equal(0.0, result.AverageRaPulseMilliseconds);
        Assert.Equal(0.0, result.AverageDecPulseMilliseconds);

        Assert.Equal(0, result.MaximumRaPulseMilliseconds);
        Assert.Equal(0, result.MaximumDecPulseMilliseconds);

        Assert.Equal(
            TimeSpan.Zero,
            result.TotalRaCorrectionTime);

        Assert.Equal(
            TimeSpan.Zero,
            result.TotalDecCorrectionTime);
    }

    private static GuidingSession CreateSession() {
        return new GuidingSession {
            Frames =
            [
                new GuideFrame
            {
                RaPulseMilliseconds = 100,
                DecPulseMilliseconds = 50
            },
            new GuideFrame
            {
                RaPulseMilliseconds = 200,
                DecPulseMilliseconds = 100
            },
            new GuideFrame
            {
                RaPulseMilliseconds = 300,
                DecPulseMilliseconds = null
            }
            ]
        };
    }
}