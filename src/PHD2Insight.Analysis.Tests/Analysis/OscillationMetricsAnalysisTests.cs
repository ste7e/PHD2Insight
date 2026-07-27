using PHD2Insight.Analysis.Metrics;
using PHD2Insight.Analysis.Tests.Builders;
using PHD2Insight.Core.Models;

namespace PHD2Insight.Analysis.Tests.Analysis;

public sealed class OscillationMetricsAnalysisTests {
    [Fact]
    public void Detect_Returns_Event_For_Single_Zero_Crossing() {
        var session = new GuidingSessionBuilder()
            .AddFrame(1, TimeSpan.Zero, raErrorArcSeconds: 1.0)
            .AddFrame(2, TimeSpan.FromSeconds(2), raErrorArcSeconds: -1.0)
            .Build();

        var events = OscillationDetector.Detect(
            (IReadOnlyList<GuideFrame>)session.Frames,
            f => f.RaErrorArcSeconds);

        var oscillation = Assert.Single(events);

        Assert.Equal(1.0, oscillation.PreviousValue);
        Assert.Equal(-1.0, oscillation.CurrentValue);
    }
    [Fact]
    public void Calculate_Returns_Expected_Metrics() {

        var builder = new GuidingSessionBuilder();

        builder.AddFrame(raErrorArcSeconds: -1, decErrorArcSeconds: 1);
        builder.AddFrame(raErrorArcSeconds: 1, decErrorArcSeconds: 2);
        builder.AddFrame(raErrorArcSeconds: -1, decErrorArcSeconds: 1);
        builder.AddFrame(raErrorArcSeconds: 1, decErrorArcSeconds: 2);

        var session = builder.Build();

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

    [Fact]
    public void Calculate_Normalises_ZeroCrossings_To_PerMinute() {
        // Arrange
        var builder = new GuidingSessionBuilder();

        for (int i = 0; i < 61; i++) {
            builder.AddFrame(
                i + 1,
                TimeSpan.FromSeconds(i * 2),
                raErrorArcSeconds: i % 2 == 0 ? 1.0 : -1.0);
        }

        var session = builder.Build();

        // Act
        var result = OscillationMetricsAnalysis.Calculate(session);

        // Assert
        Assert.Equal(30.0,
            result.RaZeroCrossingsPerMinute,
            1);
    }

    [Fact]
    public void Analyse_Returns_Zero_Crossing_Rate_For_Single_Frame() {
        // Arrange
        var builder = new GuidingSessionBuilder();

        builder.AddFrame(raErrorArcSeconds: 1.0);

        var session = builder.Build();

        // Act
        var result = OscillationMetricsAnalysis.Calculate(session);

        // Assert
        Assert.Equal(0.0, result.RaZeroCrossingsPerMinute);
        Assert.Equal(0.0, result.DecZeroCrossingsPerMinute);
        Assert.Equal(0.0, result.RaDirectionChangesPerMinute);
        Assert.Equal(0.0, result.DecDirectionChangesPerMinute);
    }

    [Fact]
    public void Analyse_Returns_Zero_Rates_For_Zero_Duration() {
        // Arrange
        var builder = new GuidingSessionBuilder();

        foreach (var raError in new double[] { 1.0, -1.0, 1.0, -1.0, 1.0 }) {
            builder.AddFrame(TimeSpan.Zero, raErrorArcSeconds: raError);
        }
        
        var session = builder.Build();

        // Act
        var result = OscillationMetricsAnalysis.Calculate(session);

        // Assert
        Assert.Equal(0.0, result.RaZeroCrossingsPerMinute);
        Assert.Equal(0.0, result.DecZeroCrossingsPerMinute);
        Assert.Equal(0.0, result.RaDirectionChangesPerMinute);
        Assert.Equal(0.0, result.DecDirectionChangesPerMinute);
    }
    private static IReadOnlyList<double> AlternateErrors(int count) {
        return Enumerable.Range(0, count)
            .Select(i => i % 2 == 0 ? 1.0 : -1.0)
            .ToArray();
    }
}