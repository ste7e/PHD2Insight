using PHD2Insight.Analysis.Diagnostics;
using PHD2Insight.Analysis.Models;
using PHD2Insight.Analysis.Observations;
using PHD2Insight.Analysis.Recommendations;

namespace PHD2Insight.Analysis.Tests.Recommendations;

public sealed class ReviewRaGuidingAggressivenessRuleTests {
    [Fact]
    public void Evaluate_WhenRaOscillationDiagnosisExists_ReturnsRecommendation() {
        var diagnosis = CreateDiagnosis("RA_OSCILLATION");

        var rule = new ReviewRaGuidingAggressivenessRule();

        var results = rule
            .Evaluate(new AnalysisResult(), new[] { diagnosis })
            .ToList();

        var recommendation = Assert.Single(results);

        Assert.Equal(
            "REVIEW_RA_GUIDING_AGGRESSIVENESS",
            recommendation.Code);

        Assert.Equal(
            "Review RA guiding aggressiveness",
            recommendation.Title);

        Assert.Equal(
            RecommendationPriority.Medium,
            recommendation.Priority);
    }

    [Fact]
    public void Evaluate_WhenRaOscillationDiagnosisDoesNotExist_ReturnsNothing() {
        var diagnosis = CreateDiagnosis("DEC_OSCILLATION");

        var rule = new ReviewRaGuidingAggressivenessRule();

        var results = rule
            .Evaluate(new AnalysisResult(), new[] { diagnosis })
            .ToList();

        Assert.Empty(results);
    }

    [Fact]
    public void Evaluate_IncludesSupportingDiagnosisAndObservations() {
        var observation = new SupportingObservation {
            Code = "HIGH_RATE_RA_OSCILLATION_EVENTS",
            Explanation = "High RA oscillation rate",
            Value = "5.0",
            Weight = 3
        };

        var diagnosis = new Diagnosis {
            Code = "RA_OSCILLATION",
            Title = "RA oscillation",
            Description = "RA oscillation detected",
            SupportingObservations = new[] { observation }
        };

        var rule = new ReviewRaGuidingAggressivenessRule();

        var recommendation = Assert.Single(
            rule.Evaluate(
                new AnalysisResult(),
                new[] { diagnosis }));

        Assert.Contains(
            "RA_OSCILLATION",
            recommendation.SupportingDiagnosisCodes);

        Assert.Contains(
            "HIGH_RATE_RA_OSCILLATION_EVENTS",
            recommendation.SupportingObservationCodes);
    }

    private static Diagnosis CreateDiagnosis(string code) =>
        new() {
            Code = code,
            Title = code,
            Description = "Test diagnosis",
            SupportingObservations =
                Array.Empty<SupportingObservation>()
        };
}