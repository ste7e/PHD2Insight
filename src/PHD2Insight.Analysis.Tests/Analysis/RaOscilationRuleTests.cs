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
                RaToDecRatio = decArcSeconds == 0
                    ? double.PositiveInfinity
                    : raArcSeconds / decArcSeconds
            },

            OscillationMetrics = new OscillationMetricsResult {
                RaZeroCrossings = 185,
                RaDirectionReversals = 170
            },

            GuideCorrections = new GuideCorrectionResult {
                AverageRaPulseMilliseconds = 245
            }
        };

        var rule = new RaOscillationRule();

        // Act
        var diagnoses = rule.Evaluate(analysis).ToList();

        // Assert
        var diagnosis = Assert.Single(diagnoses);

        Assert.Equal(DiagnosisCodes.RaOscillation, diagnosis.Code);
        Assert.Equal(DiagnosisSeverity.Warning, diagnosis.Severity);
        Assert.Equal(DiagnosisConfidence.High, diagnosis.Confidence);

        Assert.NotEmpty(diagnosis.Evidence);
    }
    [Fact]
    public void Evaluate_Returns_No_Diagnosis_When_Evidence_Score_Is_Below_Threshold() {
        // Arrange
        var analysis = new AnalysisResult {
            Rms = new RmsResult {
                RaArcSeconds = 1.8,
                DecArcSeconds = 1.2,
                RaToDecRatio = 1.5
            },

            OscillationMetrics = new OscillationMetricsResult {
                RaZeroCrossings = 120,
                RaDirectionReversals = 50
            },

            GuideCorrections = new GuideCorrectionResult {
                AverageRaPulseMilliseconds = 80
            }
        };

        var rule = new RaOscillationRule();

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
                RaArcSeconds = 1.8,
                DecArcSeconds = 1.2,
                RaToDecRatio = 1.5
            },

            OscillationMetrics = new OscillationMetricsResult {
                RaZeroCrossings = 160
            },

            GuideCorrections = new GuideCorrectionResult()
        };

        var rule = new RaOscillationRule();

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
                RaArcSeconds = 1.8,
                DecArcSeconds = 0.4,
                RaToDecRatio = 4.5
            },

            OscillationMetrics = new OscillationMetricsResult {
                RaZeroCrossings = 160
            },

            GuideCorrections = new GuideCorrectionResult()
        };

        var rule = new RaOscillationRule();

        // Act
        var diagnosis = Assert.Single(rule.Evaluate(analysis));

        // Assert
        Assert.Equal(DiagnosisConfidence.Medium, diagnosis.Confidence);
    }
    [Fact]
    public void Evaluate_Evidence_Weights_Sum_To_Confidence_Score() {
        // Arrange
        var analysis = new AnalysisResult {
            Rms = new RmsResult {
                RaArcSeconds = 1.8,
                DecArcSeconds = 0.4,
                RaToDecRatio = 4.5
            },

            OscillationMetrics = new OscillationMetricsResult {
                RaZeroCrossings = 185,
                RaDirectionReversals = 170
            },

            GuideCorrections = new GuideCorrectionResult {
                AverageRaPulseMilliseconds = 245
            }
        };

        var rule = new RaOscillationRule();

        // Act
        var diagnosis = Assert.Single(rule.Evaluate(analysis));

        // Assert
        Assert.Equal(
            10,
            diagnosis.Evidence.Sum(e => e.Weight));
    }
}