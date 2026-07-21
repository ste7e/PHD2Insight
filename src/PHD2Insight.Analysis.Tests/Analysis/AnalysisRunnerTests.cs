using PHD2Insight.Analysis.Analysis;
using PHD2Insight.Core.Models;

namespace PHD2Insight.Analysis.Tests.Analysis;

public sealed class AnalysisRunnerTests {
    [Fact]
    public void Calculate_Returns_All_Analysis_Results() {
        // Arrange
        var session = new GuidingSession {
            Frames =
            [
                new GuideFrame
                {
                    RaErrorArcSeconds = 1.0,
                    DecErrorArcSeconds = 2.0,

                    RaErrorPixels = 3.0,
                    DecErrorPixels = 4.0,

                    RaPulseMilliseconds = 100,
                    DecPulseMilliseconds = 50
                },
                new GuideFrame
                {
                    RaErrorArcSeconds = -1.0,
                    DecErrorArcSeconds = -2.0,

                    RaErrorPixels = -3.0,
                    DecErrorPixels = -4.0,

                    RaPulseMilliseconds = null,
                    DecPulseMilliseconds = 75
                }
            ]
        };

        // Act
        var result = AnalysisRunner.Calculate(session);

        // Assert
        Assert.NotNull(result);

        Assert.Equal(2, result.SessionStatistics.FrameCount);

        Assert.Equal(1.0, result.Rms.RaArcSeconds);
        Assert.Equal(2.0, result.Rms.DecArcSeconds);

        Assert.Equal(1, result.GuideCorrections.RaCorrectionCount);
        Assert.Equal(2, result.GuideCorrections.DecCorrectionCount);

        Assert.Equal(1.0, result.PeakErrors.MaximumRaErrorArcSeconds);
        Assert.Equal(2.0, result.PeakErrors.MaximumDecErrorArcSeconds);
    }

    [Fact]
    public void Calculate_Throws_For_Null_Session() {
        Assert.Throws<ArgumentNullException>(
            () => AnalysisRunner.Calculate(null!));
    }
}