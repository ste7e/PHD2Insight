using PHD2Insight.Analysis.Diagnostics;
using PHD2Insight.Analysis.Models;

namespace PHD2Insight.Analysis.Tests.Diagnostics;

public sealed class DiagnosticEngineTests {
    [Fact]
    public void Diagnose_Returns_No_Diagnoses_When_No_Rules_Are_Registered() {
        // Arrange
        var engine = new DiagnosticEngine([]);

        // Act
        var result = engine.Diagnose(new AnalysisResult());

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Diagnose_Returns_Diagnoses_From_All_Rules() {
        // Arrange
        var engine = new DiagnosticEngine(
        [
            new TestRule("TEST1"),
            new TestRule("TEST2")
        ]);

        // Act
        var result = engine.Diagnose(new AnalysisResult());

        // Assert
        Assert.Equal(2, result.Count);

        Assert.Collection(
            result,
            diagnosis => Assert.Equal("TEST1", diagnosis.Code),
            diagnosis => Assert.Equal("TEST2", diagnosis.Code));
    }

    [Fact]
    public void Diagnose_Aggregates_Multiple_Diagnoses_From_A_Single_Rule() {
        // Arrange
        var engine = new DiagnosticEngine(
        [
            new MultipleDiagnosisRule()
        ]);

        // Act
        var result = engine.Diagnose(new AnalysisResult());

        // Assert
        Assert.Equal(2, result.Count);

        Assert.Collection(
            result,
            diagnosis => Assert.Equal("FIRST", diagnosis.Code),
            diagnosis => Assert.Equal("SECOND", diagnosis.Code));
    }

    private sealed class TestRule : IDiagnosticRule {
        private readonly string _code;

        public TestRule(string code) {
            _code = code;
        }

        public IEnumerable<Diagnosis> Evaluate(AnalysisResult analysis) {
            yield return new Diagnosis {
                Code = _code
            };
        }
    }

    private sealed class MultipleDiagnosisRule : IDiagnosticRule {
        public IEnumerable<Diagnosis> Evaluate(AnalysisResult analysis) {
            yield return new Diagnosis {
                Code = "FIRST"
            };

            yield return new Diagnosis {
                Code = "SECOND"
            };
        }
    }
}