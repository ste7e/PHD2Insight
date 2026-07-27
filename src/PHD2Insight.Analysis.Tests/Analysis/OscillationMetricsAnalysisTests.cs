using PHD2Insight.Analysis.Metrics;
using PHD2Insight.Core.Models;

namespace PHD2Insight.Analysis.Tests.Analysis;

public sealed class OscillationMetricsAnalysisTests {
    [Fact]
    public void Calculate_Returns_Expected_Metrics() {
        var session = new GuidingSession {
            Frames =
            [
                new GuideFrame
                {
                    ElapsedTime = new TimeSpan(10000),
                    RaErrorArcSeconds = -1,
                    DecErrorArcSeconds = 1
                },
                new GuideFrame
                {
                    ElapsedTime = new TimeSpan(10360),
                    RaErrorArcSeconds = 1,
                    DecErrorArcSeconds = 2
                },
                new GuideFrame
                {
                    ElapsedTime = new TimeSpan(10720),
                    RaErrorArcSeconds = -1,
                    DecErrorArcSeconds = 1
                },
                new GuideFrame
                {
                    ElapsedTime = new TimeSpan(11080),
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

    private static GuidingSession CreateSession(
    int frameCount,
    TimeSpan duration,
    IReadOnlyList<double> raErrors) {
        var interval = frameCount > 1
            ? duration.TotalSeconds / (frameCount - 1)
            : 0;

        var frames = Enumerable.Range(0, frameCount)
            .Select(i => new GuideFrame {
                FrameNumber = i + 1,
                ElapsedTime = TimeSpan.FromSeconds(i * interval),

                RaErrorArcSeconds = raErrors[i],
                DecErrorArcSeconds = 0,

                // Ensure the frame is included by AnalysisFrameSelector
                RaPulseMilliseconds = 100,
                DecPulseMilliseconds = 100
            })
            .ToList();

        return new GuidingSession {
            Frames = frames,
            SettlingEvents = []
        };
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
        var session = CreateSession(
            frameCount: 61,
            duration: TimeSpan.FromMinutes(2),
            raErrors: AlternateErrors(61));

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
        var session = CreateSession(
            frameCount: 1,
            duration: TimeSpan.Zero,
            raErrors: [1.0]);

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
        var session = CreateSession(
            frameCount: 5,
            duration: TimeSpan.Zero,
            raErrors: [1.0, -1.0, 1.0, -1.0, 1.0]);

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