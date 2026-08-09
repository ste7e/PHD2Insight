using PHD2Insight.Analysis.Diagnostics;
using PHD2Insight.Analysis.Models;

namespace PHD2Insight.Analysis.Tests.Analysis;

public sealed class RaOscillationRuleTests {
    [Fact]
    public void Evaluate_Returns_Diagnosis_For_High_RA_Oscillation() {
        // Arrange
        double decArcSeconds = 0.4;
        double raArcSeconds = 1.8;
        var analysis = new AnalysisResult {
            Rms = new RmsResult {
                RaArcSeconds = raArcSeconds,
                DecArcSeconds = decArcSeconds,
            },

            OscillationMetrics = new OscillationMetricsResult {
                RaOscillationEventsPerMinute = 185,
            },

            GuideCorrections = new GuideCorrectionResult {
                AverageRaPulseMilliseconds = 245
            }
        };

        var rule = new RaOscillationDiagnosisRule();

        // Act
        var diagnoses = rule.Evaluate(analysis).ToList();

        // Assert
        var diagnosis = Assert.Single(diagnoses);

        Assert.Equal(DiagnosisCodes.RaOscillation, diagnosis.Code);
        Assert.Equal(DiagnosisSeverity.Warning, diagnosis.Severity);
        Assert.Equal(DiagnosisConfidence.High, diagnosis.Confidence);

        Assert.NotEmpty(diagnosis.SupportingObservations);
    }
    [Fact]
    public void Evaluate_Returns_No_Diagnosis_When_Evidence_Score_Is_Below_Threshold() {
        // Arrange
        var analysis = new AnalysisResult {
            Rms = new RmsResult {
                RaArcSeconds = 0.4,
                DecArcSeconds = 1.2,
            },

            OscillationMetrics = new OscillationMetricsResult {
                RaOscillationEventsPerMinute = 1.5,
            },

            GuideCorrections = new GuideCorrectionResult {
                AverageRaPulseMilliseconds = 80
            }
        };

        var rule = new RaOscillationDiagnosisRule();

        // Act
        var diagnoses = rule.Evaluate(analysis).ToList();

        // Assert
        Assert.Empty(diagnoses);
    }
    [Fact]
    public void Evaluate_Returns_Low_Confidence_For_Evidence_Score_Of_Four() {
        // Arrange
        var analysis = new AnalysisResult {
            Rms = new RmsResult {
                RaArcSeconds = 2.1,     // High RA RMS (+2)
                DecArcSeconds = 1.2,
            },

            OscillationMetrics = new OscillationMetricsResult {
                RaOscillationEventsPerMinute = 0.4,              // Below Medium threshold
                MeanRaOscillationAmplitudeArcSeconds = 1.2       // Medium amplitude (+1)
            },

            GuideCorrections = new GuideCorrectionResult {
                AverageRaPulseMilliseconds = 250                    // Large guide pulses (+1)
            }
        };
        var rule = new RaOscillationDiagnosisRule();

        // Act
        var diagnosis = Assert.Single(rule.Evaluate(analysis));

        // Assert
        Assert.Equal(DiagnosisConfidence.Low, diagnosis.Confidence);
    }
    [Fact]
    public void Evaluate_Returns_Medium_Confidence_For_Evidence_Score_Of_Seven() {
        // Arrange
        var analysis = new AnalysisResult {
            Rms = new RmsResult {
                RaArcSeconds = 1.8,     // Medium RA RMS (+2)
                DecArcSeconds = 0.4,
            },

            OscillationMetrics = new OscillationMetricsResult {
                RaOscillationEventsPerMinute = 0.4,              // Below Medium threshold
                MeanRaOscillationAmplitudeArcSeconds = 1.2       // Medium amplitude (+2)
            },

            GuideCorrections = new GuideCorrectionResult {
                AverageRaPulseMilliseconds = 45                 // No Large guide pulses
            }
        };
        var rule = new RaOscillationDiagnosisRule();

        // Act
        var diagnosis = Assert.Single(rule.Evaluate(analysis));

        Assert.Equal(7, diagnosis.SupportingObservations.Sum(o => o.Weight));
        // Assert
        Assert.Equal(DiagnosisConfidence.Medium, diagnosis.Confidence);
    }
    [Fact]
    public void Evaluate_Evidence_Weights_Sum_To_Confidence_Score() {
        // Arrange
        var analysis = new AnalysisResult {
            Rms = new RmsResult {
                RaArcSeconds = 1.8,     // High RA RMS (+2)
                DecArcSeconds = 0.4,
            },

            OscillationMetrics = new OscillationMetricsResult {
                RaOscillationEventsPerMinute = 4.0,              // Above High threshold (+3)
                MeanRaOscillationAmplitudeArcSeconds = 0.3       // Amplitude below threshold
            },

            GuideCorrections = new GuideCorrectionResult {
                AverageRaPulseMilliseconds = 245                 // Large guide pulses (+1)
            }
        };

        var rule = new RaOscillationDiagnosisRule();

        // Act
        var diagnosis = Assert.Single(rule.Evaluate(analysis));

        // Assert
        Assert.Equal(
            9,
            diagnosis.SupportingObservations.Sum(o => o.Weight));
    }
}