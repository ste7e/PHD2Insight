using PHD2Insight.Analysis.Diagnostics;
using PHD2Insight.Analysis.Models;
using PHD2Insight.Analysis.Recommendations;

namespace PHD2Insight.Analysis.Tests.Recommendations;

public sealed class RecommendationEngineTests {
    [Fact]
    public void Evaluate_RunsAllRules() {
        var rule1 = new TestRecommendationRule(
            new Recommendation {
                Code = "TEST_ONE",
                Title = "Test one",
                Description = "First recommendation"
            });

        var rule2 = new TestRecommendationRule(
            new Recommendation {
                Code = "TEST_TWO",
                Title = "Test two",
                Description = "Second recommendation"
            });

        var engine = new RecommendationEngine(
            new IRecommendationRule[] { rule1, rule2 });

        var result = engine.Evaluate(
            CreateAnalysisResult(),
            Array.Empty<Diagnosis>());

        Assert.Equal(2, result.Count);
        Assert.Equal("TEST_ONE", result[0].Code);
        Assert.Equal("TEST_TWO", result[1].Code);
    }

    [Fact]
    public void Evaluate_WithNoRules_ReturnsEmptyList() {
        var engine = new RecommendationEngine(
            Array.Empty<IRecommendationRule>());

        var result = engine.Evaluate(
            CreateAnalysisResult(),
            Array.Empty<Diagnosis>());

        Assert.Empty(result);
    }

    [Fact]
    public void Evaluate_PassesAnalysisAndDiagnosesToRules() {
        var diagnosis = new Diagnosis {
            Code = "TEST_DIAGNOSIS",
            Title = "Test diagnosis",
            Description = "Test",
            SupportingObservations = Array.Empty<SupportingObservation>()
        };

        var rule = new CapturingRecommendationRule();

        var engine = new RecommendationEngine(
            new[] { rule });

        var analysis = CreateAnalysisResult();

        engine.Evaluate(
            analysis,
            new[] { diagnosis });

        Assert.Same(analysis, rule.Analysis);
        Assert.Single(rule.Diagnoses);
        Assert.Same(diagnosis, rule.Diagnoses[0]);
    }

    private static AnalysisResult CreateAnalysisResult() =>
        new();

    private sealed class TestRecommendationRule : IRecommendationRule {
        private readonly Recommendation _recommendation;

        public TestRecommendationRule(Recommendation recommendation) {
            _recommendation = recommendation;
        }

        public IEnumerable<Recommendation> Evaluate(
            AnalysisResult analysis,
            IReadOnlyList<Diagnosis> diagnoses) {
            yield return _recommendation;
        }
    }

    private sealed class CapturingRecommendationRule : IRecommendationRule {
        public AnalysisResult? Analysis { get; private set; }

        public IReadOnlyList<Diagnosis> Diagnoses { get; private set; }
            = Array.Empty<Diagnosis>();

        public IEnumerable<Recommendation> Evaluate(
            AnalysisResult analysis,
            IReadOnlyList<Diagnosis> diagnoses) {
            Analysis = analysis;
            Diagnoses = diagnoses;

            yield break;
        }
    }

    [Fact]
    public void Constructor_WithNullRules_Throws() {
        Assert.Throws<ArgumentNullException>(
            () => new RecommendationEngine(null!));
    }

    [Fact]
    public void Evaluate_WithNullAnalysis_Throws() {
        var engine = new RecommendationEngine(
            Array.Empty<IRecommendationRule>());

        Assert.Throws<ArgumentNullException>(
            () => engine.Evaluate(null!, Array.Empty<Diagnosis>()));
    }

    [Fact]
    public void Evaluate_WithNullDiagnoses_Throws() {
        var engine = new RecommendationEngine(
            Array.Empty<IRecommendationRule>());

        Assert.Throws<ArgumentNullException>(
            () => engine.Evaluate(CreateAnalysisResult(), null!));
    }
}