using PHD2Insight.Analysis.Analysis;
using PHD2Insight.Core.Models;

namespace PHD2Insight.Analysis.Tests.Analysis;

public sealed class PeakErrorAnalysisTests {
    [Fact]
    public void Calculate_Returns_PeakErrors() {
        // Arrange
        var session = CreateSession();

        // Act
        var result = PeakErrorAnalysis.Calculate(session);

        // Assert
        Assert.Equal(5.0, result.MaximumRaErrorPixels);
        Assert.Equal(4.0, result.MaximumDecErrorPixels);
        Assert.Equal(5.0, result.MaximumTotalErrorPixels);

        Assert.Equal(1.0, result.MaximumRaErrorArcSeconds);
        Assert.Equal(0.8, result.MaximumDecErrorArcSeconds);
        Assert.Equal(1.0, result.MaximumTotalErrorArcSeconds);
    }

    [Fact]
    public void Calculate_Uses_Absolute_Error() {
        // Arrange
        var session = new GuidingSession {
            Frames =
            [
                new GuideFrame
                {
                    RaErrorPixels = -6,
                    DecErrorPixels = -8,

                    RaErrorArcSeconds = -1.2,
                    DecErrorArcSeconds = -1.6
                }
            ]
        };

        // Act
        var result = PeakErrorAnalysis.Calculate(session);

        // Assert
        Assert.Equal(6.0, result.MaximumRaErrorPixels);
        Assert.Equal(8.0, result.MaximumDecErrorPixels);
        Assert.Equal(10.0, result.MaximumTotalErrorPixels);

        Assert.Equal(1.2, result.MaximumRaErrorArcSeconds);
        Assert.Equal(1.6, result.MaximumDecErrorArcSeconds);
        Assert.Equal(2.0, result.MaximumTotalErrorArcSeconds);
    }

    [Fact]
    public void Calculate_Returns_Zero_For_Empty_Session() {
        var result = PeakErrorAnalysis.Calculate(new GuidingSession());

        Assert.Equal(0.0, result.MaximumRaErrorPixels);
        Assert.Equal(0.0, result.MaximumDecErrorPixels);
        Assert.Equal(0.0, result.MaximumTotalErrorPixels);

        Assert.Equal(0.0, result.MaximumRaErrorArcSeconds);
        Assert.Equal(0.0, result.MaximumDecErrorArcSeconds);
        Assert.Equal(0.0, result.MaximumTotalErrorArcSeconds);
    }

    private static GuidingSession CreateSession() {
        return new GuidingSession {
            Frames =
            [
                new GuideFrame
                {
                    RaErrorPixels = 5,
                    DecErrorPixels = 0,

                    RaErrorArcSeconds = 1.0,
                    DecErrorArcSeconds = 0.0
                },
                new GuideFrame
                {
                    RaErrorPixels = 3,
                    DecErrorPixels = 4,

                    RaErrorArcSeconds = 0.6,
                    DecErrorArcSeconds = 0.8
                },
                new GuideFrame
                {
                    RaErrorPixels = -2,
                    DecErrorPixels = -1,

                    RaErrorArcSeconds = -0.4,
                    DecErrorArcSeconds = -0.2
                }
            ]
        };
    }
}