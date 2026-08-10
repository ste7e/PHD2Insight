using PHD2Insight.Analysis.Diagnostics;
using PHD2Insight.Analysis.Models;
using PHD2Insight.Analysis.Observations;
using Xunit;

namespace PHD2Insight.Analysis.Tests.Observations;

public class GuideReversalDiagnosticRuleTests {
    [Fact]
    public void Evaluate_WithRaAndDecReversals_ReturnsDiagnosis() {
        // Arrange
        var analysis = new AnalysisResult {

            GuideReversals = new() {
                RaReversalRatePerMinute = 7.5,
                DecReversalRatePerMinute = 2.7
            }
        };

        var rule = new GuideReversalDiagnosisRule();

        // Act
        var diagnosis = Assert.Single(rule.Evaluate(analysis));

        // Assert
        Assert.Equal(
            DiagnosisCodes.GuideReversal,
            diagnosis.Code);

        Assert.Equal(
            DiagnosisConfidence.Low,
            diagnosis.Confidence);

        Assert.Equal(
            4,
            diagnosis.Score);
    }

    [Fact]
    public void Evaluate_WithOnlyRaReversals_ReturnsNoDiagnosis() {
        // Arrange
        var analysis = new AnalysisResult {

            GuideReversals = new() {
                RaReversalRatePerMinute = 1.5,
                DecReversalRatePerMinute = 0
            }
        };

        var rule = new GuideReversalDiagnosisRule();

        // Act
        var diagnoses = rule.Evaluate(analysis).ToList();

        // Assert
        Assert.Empty(diagnoses);
    }

    [Fact]
    public void Evaluate_WithOnlyDecReversals_ReturnsNoDiagnosis() {
        // Arrange
        var analysis = new AnalysisResult {

            GuideReversals = new() {
                RaReversalRatePerMinute = 0,
                DecReversalRatePerMinute = 1.2
            }
        };

        var rule = new GuideReversalDiagnosisRule();

        // Act
        var diagnoses = rule.Evaluate(analysis).ToList();

        // Assert
        Assert.Empty(diagnoses);
    }
}