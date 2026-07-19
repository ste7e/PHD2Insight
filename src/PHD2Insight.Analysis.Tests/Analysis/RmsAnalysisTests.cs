using PHD2Insight.Analysis.Analysis;
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
        Assert.Equal(3.0, result.RaPixels);
        Assert.Equal(4.0, result.DecPixels);
        Assert.Equal(5.0, result.TotalPixels);

        Assert.Equal(0.6, result.RaArcSeconds);
        Assert.Equal(0.8, result.DecArcSeconds);
        Assert.Equal(1.0, result.TotalArcSeconds);
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
                    RaErrorPixels = 3.0,
                    DecErrorPixels = 4.0,

                    RaErrorArcSeconds = 0.6,
                    DecErrorArcSeconds = 0.8
                }
            ]
        };
    }
}