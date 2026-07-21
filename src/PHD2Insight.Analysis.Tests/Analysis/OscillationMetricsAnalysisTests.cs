using PHD2Insight.Analysis.Metrics;
using PHD2Insight.Core.Models;

namespace PHD2Insight.Analysis.Tests.Metrics;

public sealed class OscillationMetricsAnalysisTests {
    [Fact]
    public void Calculate_Returns_Expected_Metrics() {
        var session = new GuidingSession {
            Frames =
            [
                new GuideFrame
                {
                    RaErrorArcSeconds = -1,
                    DecErrorArcSeconds = 1
                },
                new GuideFrame
                {
                    RaErrorArcSeconds = 1,
                    DecErrorArcSeconds = 2
                },
                new GuideFrame
                {
                    RaErrorArcSeconds = -1,
                    DecErrorArcSeconds = 1
                },
                new GuideFrame
                {
                    RaErrorArcSeconds = 1,
                    DecErrorArcSeconds = 2
                }
            ]
        };

        var result = OscillationMetricsAnalysis.Calculate(session);

        Assert.Equal(0.0, result.MeanRaErrorArcSeconds, 10);
        Assert.Equal(1.5, result.MeanDecErrorArcSeconds, 10);

        Assert.Equal(1.0, result.MeanAbsoluteRaErrorArcSeconds, 10);
        Assert.Equal(1.5, result.MeanAbsoluteDecErrorArcSeconds, 10);

        Assert.Equal(3, result.RaZeroCrossings);
        Assert.Equal(0, result.DecZeroCrossings);

        Assert.Equal(2, result.RaDirectionReversals);
        Assert.Equal(2, result.DecDirectionReversals);
    }

    [Fact]
    public void Calculate_Returns_Default_Result_For_Empty_Session() {
        var result = OscillationMetricsAnalysis.Calculate(
            new GuidingSession());

        Assert.Equal(0, result.RaZeroCrossings);
        Assert.Equal(0, result.DecZeroCrossings);

        Assert.Equal(0, result.RaDirectionReversals);
        Assert.Equal(0, result.DecDirectionReversals);

        Assert.Equal(0, result.MeanRaErrorArcSeconds);
        Assert.Equal(0, result.MeanDecErrorArcSeconds);
    }
}