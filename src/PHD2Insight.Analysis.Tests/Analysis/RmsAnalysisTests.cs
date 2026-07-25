using PHD2Insight.Analysis.Metrics;
using PHD2Insight.Core.Models;

namespace PHD2Insight.Analysis.Tests.Analysis;

public sealed class RmsAnalysisTests {
    [Fact]
    public void Calculate_Returns_Rms() {
        // Arrange
        var session = CreateSession();

        // Act
        var result = RmsAnalysis.Calculate(session);

        // Assert
        Assert.Equal(1.5, result.RaPixels);
        Assert.Equal(2.0, result.DecPixels);
        Assert.Equal(2.5, result.TotalPixels);

        Assert.Equal(0.3, result.RaArcSeconds);
        Assert.Equal(0.4, result.DecArcSeconds);
        Assert.Equal(0.5, result.TotalArcSeconds);
    }

    [Fact]
    public void Calculate_Returns_Zero_For_Empty_Session() {
        // Arrange
        var session = new GuidingSession();

        // Act
        var result = RmsAnalysis.Calculate(session);

        // Assert
        Assert.Equal(0.0, result.RaPixels);
        Assert.Equal(0.0, result.DecPixels);
        Assert.Equal(0.0, result.TotalPixels);

        Assert.Equal(0.0, result.RaArcSeconds);
        Assert.Equal(0.0, result.DecArcSeconds);
        Assert.Equal(0.0, result.TotalArcSeconds);
    }

    private static GuidingSession CreateSession() {
        return new GuidingSession {
            Frames =
            [
                new GuideFrame
                {
                    RaErrorPixels = 3.0,
                    DecErrorPixels = 4.0,

                    RaErrorArcSeconds = 0.6,
                    DecErrorArcSeconds = 0.8
                },

                new GuideFrame
                {
                    RaErrorPixels = 6.0,
                    DecErrorPixels = 8.0,

                    RaErrorArcSeconds = 1.2,
                    DecErrorArcSeconds = 1.6
                }
            ]
        };
    }

    [Fact]
    public void Calculate_Returns_RaToDecRatio() {
        // Arrange
        var session = new GuidingSession {
            Frames =
            [
                new GuideFrame
                {
                    RaErrorArcSeconds = 2,
                    DecErrorArcSeconds = 1
                },
                new GuideFrame
                {
                    RaErrorArcSeconds = 4,
                    DecErrorArcSeconds = 2
                }
            ]
        };

        // Act
        var result = RmsAnalysis.Calculate(session);

        // Assert
        Assert.Equal(2.0, result.RaToDecRatio);
    }

    [Fact]
    public void Calculate_Returns_Infinity_When_Dec_Rms_Is_Zero() {
        // Arrange
        var session = new GuidingSession {
            Frames =
            [
                new GuideFrame
            {
                RaErrorArcSeconds = 2,
                DecErrorArcSeconds = 0
            }
            ]
        };

        // Act
        var result = RmsAnalysis.Calculate(session);

        // Assert
        Assert.True(double.IsPositiveInfinity(result.RaToDecRatio));
    }
}