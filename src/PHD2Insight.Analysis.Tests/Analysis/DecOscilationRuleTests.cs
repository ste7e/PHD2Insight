using PHD2Insight.Analysis.Diagnostics;
using PHD2Insight.Analysis.Models;

namespace PHD2Insight.Analysis.Tests.Analysis;

public sealed class DecOscillationRuleTests {
    [Fact]
    public void Evaluate_Returns_Diagnosis_For_High_DEC_Oscillation() {
        // Arrange
        double decArcSeconds = 1.8;
        double raArcSeconds = 0.4;
        var analysis = new AnalysisResult {
            Rms = new RmsResult {
                RaArcSeconds = raArcSeconds,
                DecArcSeconds = decArcSeconds,
            },

            OscillationMetrics = new OscillationMetricsResult {
                DecOscillationEventsPerMinute = 185,
            },

            GuideCorrections = new GuideCorrectionResult {
                AverageDecPulseMilliseconds = 245
            }
        };

        var rule = new DecOscillationDiagnosisRule();

        // Act
        var diagnoses = rule.Evaluate(analysis).ToList();

        // Assert
        var diagnosis = Assert.Single(diagnoses);

        Assert.Equal(DiagnosisCodes.DecOscillation, diagnosis.Code);
        Assert.Equal(DiagnosisSeverity.Warning, diagnosis.Severity);
        Assert.Equal(DiagnosisConfidence.High, diagnosis.Confidence);

        Assert.NotEmpty(diagnosis.SupportingObservations);
    }
    [Fact]
    public void Evaluate_Returns_No_Diagnosis_When_Evidence_Score_Is_Below_Threshold() {
        // Arrange
        var analysis = new AnalysisResult {
            Rms = new RmsResult {
                RaArcSeconds = 1.2,
                DecArcSeconds = 0.4,
            },

            OscillationMetrics = new OscillationMetricsResult {
                DecOscillationEventsPerMinute = 1.5,
            },

            GuideCorrections = new GuideCorrectionResult {
                AverageDecPulseMilliseconds = 80
            }
        };

        var rule = new DecOscillationDiagnosisRule();

        // Act
        var diagnoses = rule.Evaluate(analysis).ToList();

        // Assert
        Assert.Empty(diagnoses);
    }

    // TODO:
    // Add score-based confidence tests once DEC-specific supporting
    // observations (HighDecRms, DecDominance, LargeDecGuidePulses)
    // have been implemented.

    /*    [Fact]
        public void Evaluate_Returns_Low_Confidence_For_Evidence_Score_Of_Four() {
            // Arrange
            var analysis = new AnalysisResult {
                Rms = new RmsResult {
                    RaArcSeconds = 1.2,
                    DecArcSeconds = 2.1,     // High DEC RMS (+2)
                    RaToDecRatio = 1.5      // Not RA dominant
                },

                OscillationMetrics = new OscillationMetricsResult {
                    DecOscillationEventsPerMinute = 0.4,              // Below Medium threshold
                    MeanDecOscillationAmplitudeArcSeconds = 1.2       // Medium amplitude (+1)
                },

                GuideCorrections = new GuideCorrectionResult {
                    AverageDecPulseMilliseconds = 250                    // Large guide pulses (+1)
                }
            };
            var rule = new DecOscillationDiagnosisRule();

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
                    DecArcSeconds = 1.8,     // Medium DEC RMS (+2)
                    RaArcSeconds  = 0.4,
                    RaToDecRatio = 4.5      // RA Dominance (+3)
                },

                OscillationMetrics = new OscillationMetricsResult {
                    DecOscillationEventsPerMinute = 0.4,              // Below Medium threshold
                    MeanDecOscillationAmplitudeArcSeconds = 1.2       // Medium amplitude (+2)
                },

                GuideCorrections = new GuideCorrectionResult {
                    AverageDecPulseMilliseconds = 45                 // No Large guide pulses
                }
            };
            var rule = new DecOscillationDiagnosisRule();

            // Act
            var diagnosis = Assert.Single(rule.Evaluate(analysis));

            Assert.Equal(7, diagnosis.SupportingObservations.Sum(o => o.Weight));
            // Assert
            Assert.Equal(DiagnosisConfidence.Medium, diagnosis.Confidence);
        }
    */
    [Fact]
    public void Evaluate_Evidence_Weights_Sum_To_Confidence_Score() {
        // Arrange
        var analysis = new AnalysisResult {
            Rms = new RmsResult {
                DecArcSeconds = 2.2,     // High DEC RMS (+2)
                RaArcSeconds  = 0.4,
            },  // Dec dominance (+3)

            OscillationMetrics = new OscillationMetricsResult {
                DecOscillationEventsPerMinute = 4.0,              // Above High threshold (+3)
                MeanDecOscillationAmplitudeArcSeconds = 2.3       // Amplitude above high threshold (+2)
            },

            GuideCorrections = new GuideCorrectionResult {
                AverageDecPulseMilliseconds = 245                 // Large guide pulses (+1)
            }
        };

        var rule = new DecOscillationDiagnosisRule();
        var analysisResult = rule.Evaluate(analysis).ToList();

        // Act
        var diagnosis = Assert.Single(analysisResult);

        // Assert
        Assert.Equal(
            11,
            diagnosis.SupportingObservations.Sum(o => o.Weight));
    }
}