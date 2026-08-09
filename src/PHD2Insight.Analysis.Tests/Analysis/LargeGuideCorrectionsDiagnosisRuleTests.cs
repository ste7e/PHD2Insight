using PHD2Insight.Analysis.Diagnostics;
using PHD2Insight.Analysis.Models;

namespace PHD2Insight.Analysis.Tests.Diagnostics;

public sealed class LargeGuideCorrectionsDiagnosisRuleTests {

    [Fact]
    public void Evaluate_BothAxesHaveLargeGuidePulses_ReturnsDiagnosisWithScoreOfFour() {
        // Arrange
        var analysis = new AnalysisResult {
            GuideCorrections = new GuideCorrectionResult {
                AverageRaPulseMilliseconds = 245,
                AverageDecPulseMilliseconds = 245
            }
        };

        var rule = new LargeGuideCorrectionsDiagnosisRule();

        // Act
        var diagnosis = Assert.Single(rule.Evaluate(analysis));

        // Assert
        Assert.Equal(
            DiagnosisCodes.LargeGuideCorrections,
            diagnosis.Code);

        Assert.Equal(
            4,
            diagnosis.Score);

        Assert.Equal(
            2,
            diagnosis.SupportingObservations.Count);

        Assert.Equal(
            2,
            diagnosis.SupportingObservations.Sum(o => o.Weight));
    }

    [Fact]
    public void Evaluate_OnlyRaHasLargeGuidePulses_ReturnsNoDiagnosis() {
        // Arrange
        var analysis = new AnalysisResult {
            GuideCorrections = new GuideCorrectionResult {
                AverageRaPulseMilliseconds = 245,
                AverageDecPulseMilliseconds = 100
            }
        };

        var rule = new LargeGuideCorrectionsDiagnosisRule();

        // Act
        var diagnoses = rule.Evaluate(analysis).ToList();

        // Assert
        Assert.Empty(diagnoses);
    }

    [Fact]
    public void Evaluate_OnlyDecHasLargeGuidePulses_ReturnsNoDiagnosis() {
        // Arrange
        var analysis = new AnalysisResult {
            GuideCorrections = new GuideCorrectionResult {
                AverageRaPulseMilliseconds = 100,
                AverageDecPulseMilliseconds = 245
            }
        };

        var rule = new LargeGuideCorrectionsDiagnosisRule();

        // Act
        var diagnoses = rule.Evaluate(analysis).ToList();

        // Assert
        Assert.Empty(diagnoses);
    }
}
