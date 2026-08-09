using PHD2Insight.Analysis.Diagnostics;
using PHD2Insight.Analysis.Models;
using PHD2Insight.Analysis.Observations;
using Xunit;

namespace PHD2Insight.Analysis.Tests.Observations;

public class ObservationEngineTests {

    [Fact]
    public void Create_Returns_Expected_Observations() {

        // Arrange
        var analysis = new AnalysisResult {
            Rms = new RmsResult {
                RaArcSeconds = 1.8,
                DecArcSeconds = 0.6,
            },

            OscillationMetrics = new OscillationMetricsResult {
                RaOscillationEventsPerMinute = 6.0,
                MeanRaOscillationAmplitudeArcSeconds = 2.2
            },

            GuideCorrections = new GuideCorrectionResult {
                AverageRaPulseMilliseconds = 250
            }
        };

        // Act
        var observations = ObservationEngine.Observe(analysis);

        // Assert
        Assert.Equal(5, observations.Count);

        Assert.Contains(
            observations,
            o => o.Code == ObservationCodes.HighRaRms);

        Assert.Contains(
            observations,
            o => o.Code == ObservationCodes.RaDominance);

/*        Assert.Contains(
            observations,
            o => o.Code == ObservationCodes.MediumRaOscillationRate);
*/
        Assert.Contains(
            observations,
            o => o.Code == ObservationCodes.HighRaOscillationRate);

  /*      Assert.Contains(
            observations,
            o => o.Code == ObservationCodes.MediumRaOscillationAmplitude);
*/
        Assert.Contains(
            observations,
            o => o.Code == ObservationCodes.HighRaOscillationAmplitude);

        Assert.Contains(
            observations,
            o => o.Code == ObservationCodes.LargeRaGuidePulses);
    }


    [Fact]
    public void Create_Returns_No_Observations_When_All_Metrics_Are_Normal() {

        var analysis = new AnalysisResult {
            Rms = new RmsResult {
                RaArcSeconds = 0.4,
                DecArcSeconds = 0.5,
            },

            OscillationMetrics = new OscillationMetricsResult {
                RaOscillationEventsPerMinute = 0.5,
                MeanRaOscillationAmplitudeArcSeconds = 0.2
            },

            GuideCorrections = new GuideCorrectionResult {
                AverageRaPulseMilliseconds = 50
            }
        };

        var observations = ObservationEngine.Observe(analysis);

        Assert.Collection(
            observations,
            observation => {
                Assert.Equal(
                    ObservationCodes.LowRaOscillationAmplitude,
                    observation.Code);
            },
            observation => {
                Assert.Equal(
                    ObservationCodes.NormalRaGuidePulses,
                    observation.Code);
            });
    }
    [Fact]
    public void Create_Does_Not_Return_HighRaOscillationAmplitude_Below_Threshold() {
        var analysis = new AnalysisResult {
            OscillationMetrics = new OscillationMetricsResult {
                MeanRaOscillationAmplitudeArcSeconds = 1.9
            },
            Rms = new(),
            GuideCorrections = new()
        };

        var observations = ObservationEngine.Observe(analysis);

        Assert.DoesNotContain(
            observations,
            o => o.Code == ObservationCodes.HighRaOscillationAmplitude);

        Assert.Contains(
            observations,
            o => o.Code == ObservationCodes.MediumRaOscillationAmplitude);
    }
    [Fact]
    public void Create_Returns_HighDecRms_Observation() {
        // Arrange
        var analysis = new AnalysisResult {
            Rms = new RmsResult {
                RaArcSeconds = 0.5,
                DecArcSeconds = 1.5
            },

            OscillationMetrics = new OscillationMetricsResult(),

            GuideCorrections = new GuideCorrectionResult()
        };

        // Act
        var observations = ObservationEngine.Observe(analysis);

        // Assert
        var observation = Assert.Single(observations, o => o.Code == ObservationCodes.HighDecRms);

        Assert.Equal("DEC RMS", observation.Metric);
        Assert.Equal("1.50\"", observation.Value);
        Assert.Equal(
            "DEC RMS exceeds the expected range.",
            observation.Description);
        Assert.Equal(
            ObservationWeights.HighDecRms,
            observation.Weight);
    }

    [Fact]
    public void Create_Returns_DecDominance_Observation() {
        // Arrange
        var analysis = new AnalysisResult {
            Rms = new RmsResult {
                RaArcSeconds = 0.5,
                DecArcSeconds = 1.2
            },

            OscillationMetrics = new OscillationMetricsResult(),

            GuideCorrections = new GuideCorrectionResult()
        };

        // Act
        var observations = ObservationEngine.Observe(analysis);

        // Assert
        var observation = Assert.Single(observations, o => o.Code == ObservationCodes.DecDominance);

        Assert.Equal("DEC/RA RMS Ratio", observation.Metric);
        Assert.Equal("2.40", observation.Value);
        Assert.Equal(
            "DEC guiding errors dominate RA.",
            observation.Description);
        Assert.Equal(
            ObservationWeights.DecDominance,
            observation.Weight);
    }

    [Fact]
    public void Create_Returns_LargeDecGuidePulses_Observation() {
        // Arrange
        var analysis = new AnalysisResult {
            Rms = new RmsResult(),

            OscillationMetrics = new OscillationMetricsResult(),

            GuideCorrections = new GuideCorrectionResult {
                AverageDecPulseMilliseconds = 250
            }
        };

        // Act
        var observations = ObservationEngine.Observe(analysis);

        // Assert
        var observation = Assert.Single(observations, o => o.Code == ObservationCodes.LargeDecGuidePulses);

        Assert.Equal("Average DEC Pulse", observation.Metric);
        Assert.Equal("250 ms", observation.Value);
        Assert.Equal(
            "Guide corrections are consistently large.",
            observation.Description);
        Assert.Equal(
            ObservationWeights.LargeDecGuidePulses,
            observation.Weight);
    }

    [Fact]
    public void Create_DoesNotReturn_HighDecRms_Below_Threshold() {
        var analysis = new AnalysisResult {
            Rms = new RmsResult {
                DecArcSeconds = 1.1
            },

            OscillationMetrics = new(),

            GuideCorrections = new()
        };

        var observations = ObservationEngine.Observe(analysis);

        Assert.DoesNotContain(
            observations,
            o => o.Code == ObservationCodes.HighDecRms);
    }

    [Fact]
    public void Create_DoesNotReturn_DecDominance_Below_Threshold() {
        var analysis = new AnalysisResult {
            Rms = new RmsResult {
                RaArcSeconds = 1.0,
                DecArcSeconds = 1.5
            },

            OscillationMetrics = new(),

            GuideCorrections = new()
        };

        var observations = ObservationEngine.Observe(analysis);

        Assert.DoesNotContain(
            observations,
            o => o.Code == ObservationCodes.DecDominance);
    }

    [Fact]
    public void Create_DoesNotReturn_LargeDecGuidePulses_Below_Threshold() {
        var analysis = new AnalysisResult {
            Rms = new(),

            OscillationMetrics = new(),

            GuideCorrections = new GuideCorrectionResult {
                AverageDecPulseMilliseconds = 150
            }
        };

        var observations = ObservationEngine.Observe(analysis);

        Assert.DoesNotContain(
            observations,
            o => o.Code == ObservationCodes.LargeDecGuidePulses);
    }

    [Fact]
    public void Create_Returns_OccasionalLostStars_Observation() {
        var analysis = new AnalysisResult {
            LostStars = new LostStarResult {
                LostStarCount = 1,
                LostStarPercentage = 1.0
            }
        };

        var observations = ObservationEngine.Observe(analysis);

        var observation = Assert.Single(observations, o => o.Code == ObservationCodes.OccasionalLostStars);

        Assert.Equal("1.0%", observation.Value);
        Assert.Equal(ObservationWeights.OccasionalLostStars, observation.Weight);
    }
    [Fact]
    public void Create_Returns_FrequentLostStars_Observation() {
        var analysis = new AnalysisResult {
            LostStars = new LostStarResult {
                LostStarCount = 5,
                LostStarPercentage = 5.0
            }
        };

        var observations = ObservationEngine.Observe(analysis);

        var observation = Assert.Single(observations, o => o.Code == ObservationCodes.FrequentLostStars);

        Assert.Equal("5.0%", observation.Value);
        Assert.Equal(ObservationWeights.FrequentLostStars, observation.Weight);
    }
    [Fact]
    public void Create_Returns_SevereLostStars_Observation() {
        var analysis = new AnalysisResult {
            LostStars = new LostStarResult {
                LostStarCount = 20,
                LostStarPercentage = 20.0
            }
        };

        var observations = ObservationEngine.Observe(analysis);

        var observation = Assert.Single(observations, o => o.Code == ObservationCodes.SevereLostStars);

        Assert.Equal("20.0%", observation.Value);
        Assert.Equal(ObservationWeights.SevereLostStars, observation.Weight);
    }
    [Fact]
    public void Evaluate_Reduces_Confidence_When_Severe_Lost_Stars_Are_Present() {
        // Arrange
        var analysis = new AnalysisResult {
            Rms = new RmsResult {
                RaArcSeconds = 1.0,   // Below 1.2" threshold
                DecArcSeconds = 0.2   // Ratio = 5.0 (RA dominance still fires)
            },

            OscillationMetrics = new OscillationMetricsResult {
                RaOscillationEventsPerMinute = 1.5,
                MeanRaOscillationAmplitudeArcSeconds = 0.8
            },

            LostStars = new LostStarResult {
                LostStarCount = 50,
                LostStarPercentage = 25.0
            },

            GuideCorrections = new GuideCorrectionResult()
        };

        var rule = new RaOscillationDiagnosisRule();

        // Act
        var diagnosis = Assert.Single(rule.Evaluate(analysis));

        // Assert
        Assert.Equal(DiagnosisConfidence.Low, diagnosis.Confidence);
        Assert.Equal(5, diagnosis.Score);
    }

    [Fact]
    public void Create_Returns_Low_Ra_Oscillation_Amplitude_Observation() {
        var analysis = new AnalysisResult {
            OscillationMetrics = new OscillationMetricsResult {
                MeanRaOscillationAmplitudeArcSeconds = 0.30
            }
        };

        var observations = ObservationEngine.Observe(analysis);

        var observation = Assert.Single(observations, o =>
                o.Code == ObservationCodes.LowRaOscillationAmplitude);

        Assert.Equal("0.30\"", observation.Value);
        Assert.Equal(
            ObservationWeights.LowRaOscillationAmplitude,
            observation.Weight);
    }

    [Fact]
    public void Create_Does_Not_Return_Low_Ra_Oscillation_Amplitude_When_Amplitude_Is_Zero() {
        var analysis = new AnalysisResult {
            OscillationMetrics = new OscillationMetricsResult {
                MeanRaOscillationAmplitudeArcSeconds = 0.0
            }
        };

        var observations = ObservationEngine.Observe(analysis);

        Assert.DoesNotContain(
            observations,
            o => o.Code == ObservationCodes.LowRaOscillationAmplitude);
    }

    [Fact]
    public void Create_Returns_Low_Dec_Oscillation_Amplitude_Observation() {
        var analysis = new AnalysisResult {
            OscillationMetrics = new OscillationMetricsResult {
                MeanDecOscillationAmplitudeArcSeconds = 0.30
            }
        };

        var observations = ObservationEngine.Observe(analysis);

        var observation = Assert.Single(observations, o =>
                o.Code == ObservationCodes.LowDecOscillationAmplitude);

        Assert.Equal("0.30\"", observation.Value);
        Assert.Equal(
            ObservationWeights.LowDecOscillationAmplitude,
            observation.Weight);
    }

    [Fact]
    public void Create_Does_Not_Return_Low_Dec_Oscillation_Amplitude_When_Amplitude_Is_Zero() {
        var analysis = new AnalysisResult {
            OscillationMetrics = new OscillationMetricsResult {
                MeanDecOscillationAmplitudeArcSeconds = 0.0
            }
        };

        var observations = ObservationEngine.Observe(analysis);

        Assert.DoesNotContain(
            observations,
            o => o.Code == ObservationCodes.LowDecOscillationAmplitude);
    }
    [Fact]
    public void Evaluate_Returns_Diagnosis_For_Severe_Lost_Stars() {
        var analysis = new AnalysisResult {
            LostStars = new LostStarResult {
                LostStarCount = 80,
                LostStarPercentage = 40
            },

            OscillationMetrics = new OscillationMetricsResult {
                MeanRaOscillationAmplitudeArcSeconds = 0.3,
                MeanDecOscillationAmplitudeArcSeconds = 0.3
            },

            GuideCorrections = new GuideCorrectionResult {
                AverageRaPulseMilliseconds = 50,
                AverageDecPulseMilliseconds = 60
            }
        };

        var rule = new PoorTransparencyDiagnosisRule();

        var diagnosis = Assert.Single(rule.Evaluate(analysis));

        Assert.Equal(
            DiagnosisCodes.PoorTransparency,
            diagnosis.Code);

        Assert.Equal(DiagnosisConfidence.Medium, diagnosis.Confidence);
        Assert.Equal(7, diagnosis.Score);
    }
    [Fact]
    public void Evaluate_Returns_Diagnosis_For_Aggressive_Guiding() {
        var analysis = new AnalysisResult {

            OscillationMetrics = new() {

                RaOscillationEventsPerMinute = 6.0,

                MeanRaOscillationAmplitudeArcSeconds = 0.8,

                DecOscillationEventsPerMinute = 5.0,

                MeanDecOscillationAmplitudeArcSeconds = 0.7
            },

            GuideCorrections = new() {

                AverageRaPulseMilliseconds = 50,

                AverageDecPulseMilliseconds = 55
            }
        };

        var rule = new AggressiveGuidingDiagnosisRule();

        var diagnosis = Assert.Single(rule.Evaluate(analysis));

        Assert.Equal(
            DiagnosisCodes.AggressiveGuiding,
            diagnosis.Code);

        Assert.Equal(DiagnosisConfidence.Medium, diagnosis.Confidence);
        Assert.Equal(7, diagnosis.Score);
    }
}