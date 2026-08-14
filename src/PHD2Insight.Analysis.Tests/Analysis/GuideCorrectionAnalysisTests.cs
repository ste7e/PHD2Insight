using PHD2Insight.Analysis.Metrics;
using PHD2Insight.Analysis.Tests.Builders;
using PHD2Insight.Core.Models;

namespace PHD2Insight.Analysis.Tests.Analysis;

public sealed class GuideCorrectionAnalysisTests {

    [Fact]
    public void Calculate_Returns_GuideCorrectionStatistics() {
        // Arrange
        var session = new GuidingSessionBuilder()
            .AddFrame(TimeSpan.Zero, raPulseMilliseconds: 100, decPulseMilliseconds: 50)
            .AddFrame(TimeSpan.Zero, raPulseMilliseconds: 200, decPulseMilliseconds: 100)
            .AddFrame(TimeSpan.Zero, raPulseMilliseconds: 300, decPulseMilliseconds: null)
            .Build();

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
        var session = new GuidingSessionBuilder()
            .AddFrame(TimeSpan.Zero, raPulseMilliseconds: null)
            .AddFrame(TimeSpan.Zero, raPulseMilliseconds: 100)
            .AddFrame(TimeSpan.Zero, raPulseMilliseconds: null)
            .AddFrame(TimeSpan.Zero, raPulseMilliseconds: 300)
            .Build();

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

    [Fact]
    public void Calculate_CountsGuideCorrectionsByDirection() {
        var session = new GuidingSessionBuilder()
            .AddFrame(new GuideFrame {
                FrameNumber = 1,
                ElapsedTime = TimeSpan.FromSeconds(0),
                RaPulseMilliseconds = 100,
                RaDirection = GuideDirection.East,
                DecPulseMilliseconds = 100,
                DecDirection = GuideDirection.North
            })
            .AddFrame(new GuideFrame {
                FrameNumber = 2,
                ElapsedTime = TimeSpan.FromSeconds(1),
                RaPulseMilliseconds = 100,
                RaDirection = GuideDirection.West,
                DecPulseMilliseconds = 100,
                DecDirection = GuideDirection.North
            })
            .AddFrame(new GuideFrame {
                FrameNumber = 3,
                ElapsedTime = TimeSpan.FromSeconds(2),
                RaPulseMilliseconds = 100,
                RaDirection = GuideDirection.East,
                DecPulseMilliseconds = 100,
                DecDirection = GuideDirection.South
            })
            .AddFrame(new GuideFrame {
                FrameNumber = 4,
                ElapsedTime = TimeSpan.FromSeconds(3),
                RaPulseMilliseconds = 100,
                RaDirection = GuideDirection.East,
                DecPulseMilliseconds = null,
                DecDirection = GuideDirection.None
            })
            .AddFrame(new GuideFrame {
                FrameNumber = 5,
                ElapsedTime = TimeSpan.FromSeconds(4),
                RaPulseMilliseconds = null,
                RaDirection = GuideDirection.None,
                DecPulseMilliseconds = 100,
                DecDirection = GuideDirection.South
            })
            .Build();

        var result = GuideCorrectionAnalysis.Calculate(session);

        Assert.Equal(3, result.RaEastCorrectionCount);
        Assert.Equal(1, result.RaWestCorrectionCount);

        Assert.Equal(2, result.DecNorthCorrectionCount);
        Assert.Equal(2, result.DecSouthCorrectionCount);
    }

    [Fact]
    public void Calculate_ReturnsZeroDirectionalImbalance_WhenCorrectionsAreBalanced() {
        // Build a session with equal East/West and North/South corrections.

        var session = new GuidingSessionBuilder()
            .AddFrame(new GuideFrame {
                FrameNumber = 1,
                ElapsedTime = TimeSpan.FromSeconds(0),
                RaPulseMilliseconds = 100,
                RaDirection = GuideDirection.East,
                DecPulseMilliseconds = 100,
                DecDirection = GuideDirection.North
            })
            .AddFrame(new GuideFrame {
                FrameNumber = 1,
                ElapsedTime = TimeSpan.FromSeconds(1),
                RaPulseMilliseconds = 100,
                RaDirection = GuideDirection.West,
                DecPulseMilliseconds = 100,
                DecDirection = GuideDirection.South
            })
            .AddFrame(new GuideFrame {
                FrameNumber = 1,
                ElapsedTime = TimeSpan.FromSeconds(2),
                RaPulseMilliseconds = 100,
                RaDirection = GuideDirection.East,
                DecPulseMilliseconds = 100,
                DecDirection = GuideDirection.South
            })
            .AddFrame(new GuideFrame {
                FrameNumber = 1,
                ElapsedTime = TimeSpan.FromSeconds(3),
                RaPulseMilliseconds = 100,
                RaDirection = GuideDirection.West,
                DecPulseMilliseconds = 100,
                DecDirection = GuideDirection.North
            })
            .Build();
        var result = GuideCorrectionAnalysis.Calculate(session);

        Assert.Equal(0.0, result.RaDirectionalImbalance);
        Assert.Equal(0.0, result.DecDirectionalImbalance);
    }

    [Fact]
    public void Calculate_ReturnsExpectedDirectionalImbalance_WhenCorrectionsAreUnbalanced() {
        // 3 East, 1 West
        // 3 North, 1 South

        var session = new GuidingSessionBuilder()
            .AddFrame(new GuideFrame {
                FrameNumber = 1,
                ElapsedTime = TimeSpan.FromSeconds(0),
                RaPulseMilliseconds = 100,
                RaDirection = GuideDirection.East,
                DecPulseMilliseconds = 100,
                DecDirection = GuideDirection.North
            })
            .AddFrame(new GuideFrame {
                FrameNumber = 1,
                ElapsedTime = TimeSpan.FromSeconds(1),
                RaPulseMilliseconds = 100,
                RaDirection = GuideDirection.East,
                DecPulseMilliseconds = 100,
                DecDirection = GuideDirection.South
            })
            .AddFrame(new GuideFrame {
                FrameNumber = 1,
                ElapsedTime = TimeSpan.FromSeconds(2),
                RaPulseMilliseconds = 100,
                RaDirection = GuideDirection.East,
                DecPulseMilliseconds = 100,
                DecDirection = GuideDirection.North
            })
            .AddFrame(new GuideFrame {
                FrameNumber = 1,
                ElapsedTime = TimeSpan.FromSeconds(3),
                RaPulseMilliseconds = 100,
                RaDirection = GuideDirection.West,
                DecPulseMilliseconds = 100,
                DecDirection = GuideDirection.North
            })
            .Build();
        var result = GuideCorrectionAnalysis.Calculate(session);

        Assert.Equal(0.5, result.RaDirectionalImbalance);
        Assert.Equal(0.5, result.DecDirectionalImbalance);
    }
}