using PHD2Insight.Analysis.Detection;
using PHD2Insight.Analysis.Frequency;
using PHD2Insight.Analysis.Metrics;
using PHD2Insight.Analysis.Models;
using PHD2Insight.Analysis.Observations;
using PHD2Insight.Analysis.Tests.Builders;
using PHD2Insight.Core.Models;

namespace PHD2Insight.Analysis.Tests.Analysis;

public sealed class OscillationMetricsAnalysisTests {
    [Fact]
    public void Calculate_Returns_Expected_Metrics() {

        var builder = new GuidingSessionBuilder();

        builder.AddFrame(raErrorArcSeconds: -1, decErrorArcSeconds: 1);
        builder.AddFrame(raErrorArcSeconds: 1, decErrorArcSeconds: 2);
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

        Assert.Equal(24, result.RaOscillationEventsPerMinute);
        Assert.Equal(0, result.DecOscillationEventsPerMinute);

        Assert.Equal(4, result.RaDirectionReversals);
        Assert.Equal(4, result.DecDirectionReversals);
    }

    [Fact]
    public void Calculate_Returns_Default_Result_For_Empty_Session() {
        var result = OscillationMetricsAnalysis.Calculate(
            new GuidingSession());

        Assert.Equal(0, result.RaOscillationEventsPerMinute);
        Assert.Equal(0, result.DecOscillationEventsPerMinute);

        Assert.Equal(0, result.RaDirectionReversals);
        Assert.Equal(0, result.DecDirectionReversals);

        Assert.Equal(0, result.MeanRaErrorArcSeconds);
        Assert.Equal(0, result.MeanDecErrorArcSeconds);
    }

    [Fact]
    public void Calculate_Normalises_ZeroCrossings_To_PerMinute() {
        // Arrange
        var session = new GuidingSessionBuilder()
            .AddFrame(1)
            .AddFrame(2)
            .AddFrame(3)
            .AddFrame(2)
            .AddFrame(1)
            .AddFrame(-1)
            .AddFrame(-2)
            .AddFrame(-3)
            .AddFrame(-2)
            .AddFrame(-1)

            .AddFrame(1)
            .AddFrame(2)
            .AddFrame(3)
            .AddFrame(2)
            .AddFrame(1)
            .AddFrame(-1)
            .AddFrame(-2)
            .AddFrame(-3)
            .AddFrame(-2)
            .AddFrame(-1)

            .AddFrame(1)
            .AddFrame(2)
            .AddFrame(3)
            .AddFrame(2)
            .AddFrame(1)
            .AddFrame(-1)
            .AddFrame(-2)
            .AddFrame(-3)
            .AddFrame(-2)
            .AddFrame(-1)

            .AddFrame(1)
            .Build();

        // Act
        var result = OscillationMetricsAnalysis.Calculate(session);

        // Assert
        Assert.Equal(5.0,
            result.RaOscillationEventsPerMinute,
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
        Assert.Equal(0.0, result.RaOscillationEventsPerMinute);
        Assert.Equal(0.0, result.DecOscillationEventsPerMinute);
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
        Assert.Equal(0.0, result.RaOscillationEventsPerMinute);
        Assert.Equal(0.0, result.DecOscillationEventsPerMinute);
        Assert.Equal(0.0, result.RaDirectionChangesPerMinute);
        Assert.Equal(0.0, result.DecDirectionChangesPerMinute);
    }

    [Fact]
    public void Detect_Returns_One_Peak_For_One_Positive_Excursion() {
        var session = new GuidingSessionBuilder()
            .AddFrame(1, TimeSpan.Zero, 0.4)
            .AddFrame(2, TimeSpan.FromSeconds(2), 1.1)
            .AddFrame(3, TimeSpan.FromSeconds(4), 0.8)
            .AddFrame(4, TimeSpan.FromSeconds(6), 1.5)
            .AddFrame(5, TimeSpan.FromSeconds(8), 1.2)
            .AddFrame(6, TimeSpan.FromSeconds(10), -0.2)
            .Build();

        var peaks = PeakDetector.Detect(
            session.Frames,
            f => f.RaErrorArcSeconds);

        var peak = Assert.Single(peaks);

        Assert.Equal(1.5, peak.Value);
    }

    [Fact]
    public void Detect_Returns_Two_Peaks_For_Two_Excursions() {
        var session = new GuidingSessionBuilder()
            .AddFrame(1, TimeSpan.Zero, 0.4)
            .AddFrame(2, TimeSpan.FromSeconds(2), 1.1)
            .AddFrame(3, TimeSpan.FromSeconds(4), 1.5)
            .AddFrame(4, TimeSpan.FromSeconds(6), 0.7)
            .AddFrame(5, TimeSpan.FromSeconds(8), -0.2)
            .AddFrame(6, TimeSpan.FromSeconds(10), -1.3)
            .AddFrame(7, TimeSpan.FromSeconds(12), -0.8)
            .AddFrame(8, TimeSpan.FromSeconds(14), 0.2)
            .Build();

        var peaks = PeakDetector.Detect(
            session.Frames,
            f => f.RaErrorArcSeconds);

        Assert.Equal(2, peaks.Count);

        Assert.Equal(1.5, peaks[0].Value);
        Assert.Equal(-1.3, peaks[1].Value);
    }

    [Fact]
    public void Detect_DoesNotEmitIncompleteFinalExcursion() {
        var session = new GuidingSessionBuilder()
            .AddFrame(1)
            .AddFrame(2)
            .AddFrame(3)
            .Build();

        var peaks = PeakDetector.Detect(
            session.Frames,
            f => f.RaErrorArcSeconds);

        Assert.Empty(peaks);
    }
    [Fact]
    public void Detect_EmitsPeakWhenExcursionCompletes() {
        var session = new GuidingSessionBuilder()
            .AddFrame(1)
            .AddFrame(2)
            .AddFrame(3)
            .AddFrame(-1)
            .Build();

        var peaks = PeakDetector.Detect(
            session.Frames,
            f => f.RaErrorArcSeconds);

        var peak = Assert.Single(peaks);

        Assert.Equal(3, peak.Value);
    }

    [Fact]
    public void Calculate_WithTooFewFrames_ReturnsInvalidMechanicalPeriodPower() {
        var session = new GuidingSessionBuilder()
            .AddFrame(TimeSpan.FromSeconds(0))
            .AddFrame(TimeSpan.FromSeconds(2))
            .AddFrame(TimeSpan.FromSeconds(4))
            .AddFrame(TimeSpan.FromSeconds(6))
            .AddFrame(TimeSpan.FromSeconds(8))
            .AddFrame(TimeSpan.FromSeconds(10))
            .AddFrame(TimeSpan.FromSeconds(12))
            .AddFrame(TimeSpan.FromSeconds(14))
            .AddFrame(TimeSpan.FromSeconds(16))
            .Build();

        var result = OscillationMetricsAnalysis.Calculate(session);

        Assert.False(result.MechanicalPeriodPower.IsValid);
    }

    [Fact]
    public void Calculate_WithSufficientFrames_ReturnsMechanicalPeriodPower() {
        var session = new GuidingSessionBuilder();

        for (var i = 0; i < 50; i++) {
            var elapsedTime = TimeSpan.FromSeconds(i * 10);

            session.AddFrame(new GuideFrame {
                FrameNumber = i + 1,
                ElapsedTime = elapsedTime,
                RaGuideDistance = Math.Sin(
                    2 * Math.PI * elapsedTime.TotalSeconds /
                    MountPeriodProfiles.Eq6RPro.RaPeriods
                        .First(p => p.Name == "RA worm fundamental")
                        .PeriodSeconds),
                RaErrorArcSeconds = 0,
                DecErrorArcSeconds = 0,
                RaPulseMilliseconds = 100,
                DecPulseMilliseconds = 100
            });
        }

        var result =
            OscillationMetricsAnalysis.Calculate(session.Build());

        Assert.True(result.MechanicalPeriodPower.IsValid);

        Assert.True(
            double.IsFinite(
                result.MechanicalPeriodPower.RaWormFundamentalPower));

        Assert.Equal(
            MountPeriodProfiles.Eq6RPro.RaPeriods
                .First(p => p.Name == "RA worm fundamental")
                .PeriodSeconds,
            result.MechanicalPeriodPower.RaWormPeriodSeconds);


        Assert.InRange(
            result.MechanicalPeriodPower.RaWormFundamentalArcSeconds,
            0.99,
            1.01);

    }

    [Fact]
    public void EvaluateAmplitude_AtKnownFrequency_ReturnsExpectedAmplitude() {
        const double expectedAmplitude = 3.5;

        var periodSeconds =
            MountPeriodProfiles.Eq6RPro.RaPeriods
                .First(p => p.Name == "RA worm fundamental")
                .PeriodSeconds;

        var frequencyHz = 1.0 / periodSeconds;

        var times = Enumerable
            .Range(0, 200)
            .Select(i => i * 5.0)
            .ToArray();

        var values = times
            .Select(t =>
                expectedAmplitude *
                Math.Sin(2.0 * Math.PI * frequencyHz * t))
            .ToArray();

        var amplitude =
            LombScarglePeriodogram.EvaluateAmplitude(
                times,
                values,
                frequencyHz);

        Assert.NotNull(amplitude);

        Assert.InRange(
            amplitude.Value,
            expectedAmplitude - 0.01,
            expectedAmplitude + 0.01);
    }
    [Fact]
    public void PecRecommendation_WhenWormAmplitudeBelowThreshold_IsNotObserved() {
        var analysis = new AnalysisResult {
            OscillationMetrics = new OscillationMetricsResult {
                MechanicalPeriodPower = new MechanicalPeriodPowerResult {
                    RaWormFundamentalArcSeconds =
                    DiagnosisThresholds.PecRecommendationRaWormAmplitudeArcSeconds - 0.01,
                    RaWormFundamentalPower = 3.0,
                    RaWormPeriodSeconds = 478.0,
                    IsValid = true
                }
            }
        };

        var observations = ObservationEngine.Observe(analysis);

        Assert.DoesNotContain(
            observations,
            o => o.Code == ObservationCodes.PecRecommended);
    }

    [Fact]
    public void PecRecommendation_WhenWormAmplitudeExceedsThreshold_IsObserved() {
        var amplitude =
            DiagnosisThresholds.PecRecommendationRaWormAmplitudeArcSeconds + 0.01;

        var analysis = new AnalysisResult {
            OscillationMetrics = new OscillationMetricsResult {
                MechanicalPeriodPower = new MechanicalPeriodPowerResult {
                    RaWormFundamentalArcSeconds = amplitude,
                    RaWormFundamentalPower = 3.0,
                    RaWormPeriodSeconds = 478.0,
                    IsValid = true
                }
            }
        };

        var observations = ObservationEngine.Observe(analysis);

        var observation = Assert.Single(observations, o => o.Code == ObservationCodes.PecRecommended);

        Assert.Equal(
            $"{amplitude:F2}\"",
            observation.Value);
    }

    [Fact]
    public void PecRecommendation_WhenWormAmplitudeEqualsThreshold_IsNotObserved() {
        var amplitude =
            DiagnosisThresholds.PecRecommendationRaWormAmplitudeArcSeconds;

        var analysis = new AnalysisResult {
            OscillationMetrics = new OscillationMetricsResult {
                MechanicalPeriodPower = new MechanicalPeriodPowerResult {
                    RaWormFundamentalArcSeconds = amplitude,
                    RaWormFundamentalPower = 3.0,
                    RaWormPeriodSeconds = 478.0,
                    IsValid = true
                }
            }
        };

        var observations = ObservationEngine.Observe(analysis);

        Assert.DoesNotContain(
            observations,
            o => o.Code == ObservationCodes.PecRecommended);
    }

    [Fact]
    public void PecRecommendation_WhenMechanicalPeriodPowerIsInvalid_IsNotObserved() {
        var analysis = new AnalysisResult {
            OscillationMetrics = new OscillationMetricsResult {
                MechanicalPeriodPower = new MechanicalPeriodPowerResult {
                    RaWormFundamentalArcSeconds = 10.0,
                    RaWormFundamentalPower = 10.0,
                    RaWormPeriodSeconds = 478.0,
                    IsValid = false
                }
            }
        };

        var observations = ObservationEngine.Observe(analysis);

        Assert.DoesNotContain(
            observations,
            o => o.Code == ObservationCodes.PecRecommended);
    }
}