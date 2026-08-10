using PHD2Insight.Analysis.Diagnostics;
using PHD2Insight.Analysis.Models;

namespace PHD2Insight.Analysis.Tests.Diagnostics;

public sealed class AggressiveGuidingDiagnosisRuleTests {

    [Fact]
    public void Evaluate_Returns_Diagnosis_For_Aggressive_Guiding() {
        // Arrange
        var analysis = new AnalysisResult {

            OscillationMetrics = new() {

                RaOscillationEventsPerMinute = 15.0,
                MeanRaOscillationAmplitudeArcSeconds = 0.8,

                DecOscillationEventsPerMinute = 10.0,
                MeanDecOscillationAmplitudeArcSeconds = 0.7
            },

            GuideCorrections = new() {

                AverageRaPulseMilliseconds = 250,
                AverageDecPulseMilliseconds = 250
            }
        };

        var rule = new AggressiveGuidingDiagnosisRule();

        // Act
        var diagnosis = Assert.Single(rule.Evaluate(analysis));

        // Assert
        Assert.Equal(
            DiagnosisCodes.AggressiveGuiding,
            diagnosis.Code);

        Assert.Equal(
            DiagnosisConfidence.High,
            diagnosis.Confidence);

        Assert.Equal(
            9,
            diagnosis.Score);
    }

    [Fact]
    public void Evaluate_Returns_Diagnosis_For_HighRaRateAndMediumRaAmplitude() {
        // Arrange
        var analysis = new AnalysisResult {

            OscillationMetrics = new() {

                RaOscillationEventsPerMinute = 15.0,
                MeanRaOscillationAmplitudeArcSeconds = 0.95
            }
        };

        var rule = new AggressiveGuidingDiagnosisRule();

        // Act
        var diagnosis = Assert.Single(rule.Evaluate(analysis));

        // Assert
        Assert.Equal(
            DiagnosisCodes.AggressiveGuiding,
            diagnosis.Code);

        Assert.Equal(
            DiagnosisConfidence.Low,
            diagnosis.Confidence);

        Assert.Equal(
            5,
            diagnosis.Score);
    }

    [Fact]
    public void Evaluate_Returns_Diagnosis_For_HighDecRateAndMediumDecAmplitude() {
        // Arrange
        var analysis = new AnalysisResult {

            OscillationMetrics = new() {

                DecOscillationEventsPerMinute = 10.0,
                MeanDecOscillationAmplitudeArcSeconds = 0.7
            }
        };

        var rule = new AggressiveGuidingDiagnosisRule();

        // Act
        var diagnosis = Assert.Single(rule.Evaluate(analysis));

        // Assert
        Assert.Equal(
            DiagnosisCodes.AggressiveGuiding,
            diagnosis.Code);

        Assert.Equal(
            DiagnosisConfidence.Low,
            diagnosis.Confidence);

        Assert.Equal(
            4,
            diagnosis.Score);
    }

    [Fact]
    public void Evaluate_Returns_Diagnosis_For_HighRaAndDecRates() {
        // Arrange
        var analysis = new AnalysisResult {

            OscillationMetrics = new() {

                RaOscillationEventsPerMinute = 15.0,
                DecOscillationEventsPerMinute = 10.0    
            }
        };

        var rule = new AggressiveGuidingDiagnosisRule();

        // Act
        var diagnosis = Assert.Single(rule.Evaluate(analysis));

        // Assert
        Assert.Equal(
            DiagnosisCodes.AggressiveGuiding,
            diagnosis.Code);

        Assert.Equal(
            DiagnosisConfidence.Medium,
            diagnosis.Confidence);

        Assert.Equal(
            6,
            diagnosis.Score);
    }

    [Fact]
    public void Evaluate_WithOnlyLargeGuidePulses_ReturnsNoDiagnosis() {
        // Arrange
        var analysis = new AnalysisResult {

            GuideCorrections = new() {

                AverageRaPulseMilliseconds = 250,
                AverageDecPulseMilliseconds = 250
            }
        };

        var rule = new AggressiveGuidingDiagnosisRule();

        // Act
        var diagnoses = rule.Evaluate(analysis).ToList();

        // Assert
        Assert.Empty(diagnoses);
    }

    [Fact]
    public void Evaluate_WhenEvidenceScoreIsBelowMinimum_ReturnsNoDiagnosis() {
        // Arrange
        var analysis = new AnalysisResult {

            OscillationMetrics = new() {

                RaOscillationEventsPerMinute = 4.0
            }
        };

        var rule = new AggressiveGuidingDiagnosisRule();

        // Act
        var diagnoses = rule.Evaluate(analysis).ToList();

        // Assert
        Assert.Empty(diagnoses);
    }
    [Fact]
    public void Evaluate_WithHighRaRateAndMediumDecAmplitude_ReturnsNoDiagnosis() {
        // Arrange
        var analysis = new AnalysisResult {

            OscillationMetrics = new() {

                RaOscillationEventsPerMinute = 4.0,
                MeanDecOscillationAmplitudeArcSeconds = 0.7
            }
        };

        var rule = new AggressiveGuidingDiagnosisRule();

        // Act
        var diagnoses = rule.Evaluate(analysis).ToList();

        // Assert
        Assert.Empty(diagnoses);
    }

    [Fact]
    public void Evaluate_WithHighDecRateAndMediumRaAmplitude_ReturnsNoDiagnosis() {
        // Arrange
        var analysis = new AnalysisResult {

            OscillationMetrics = new() {

                DecOscillationEventsPerMinute = 4.0,
                MeanRaOscillationAmplitudeArcSeconds = 0.8
            }
        };

        var rule = new AggressiveGuidingDiagnosisRule();

        // Act
        var diagnoses = rule.Evaluate(analysis).ToList();

        // Assert
        Assert.Empty(diagnoses);
    }


}
