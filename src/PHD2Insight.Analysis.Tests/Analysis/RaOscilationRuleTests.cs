using PHD2Insight.Analysis.Diagnostics;
using PHD2Insight.Analysis.Models;

namespace PHD2Insight.Analysis.Tests.Diagnostics;

public sealed class RaOscillationRuleTests {
    [Fact]
    public void Evaluate_Returns_Diagnosis_For_High_RA_Oscillation() {
        // Arrange
        var analysis = new AnalysisResult {
            Rms = new RmsResult {
                RaArcSeconds = 1.8,
                DecArcSeconds = 0.4
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
}