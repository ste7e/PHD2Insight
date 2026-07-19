using PHD2Insight.Core.Models;
using PHD2Insight.Parser.Parsers;

namespace PHD2Insight.Tests.Parsers;

public sealed class GuideAlgorithmContinuationLineParserTests {
    [Fact]
    public void Parses_prediction_gain() {
        var algorithm = new GuideAlgorithmInfo();

        var result =
            GuideAlgorithmContinuationLineParser.TryApply(
                "Prediction gain = 0.350",
                algorithm);

        Assert.True(result);

        Assert.Equal(
            0.350,
            algorithm.PredictionGain);
    }

    [Fact]
    public void Parses_aggression() {
        var algorithm = new GuideAlgorithmInfo();

        var result =
            GuideAlgorithmContinuationLineParser.TryApply(
                "Aggression = 70%",
                algorithm);

        Assert.True(result);

        Assert.Equal(
            70.0,
            algorithm.Aggression);
    }

    [Fact]
    public void Returns_false_for_unknown_property() {
        var algorithm = new GuideAlgorithmInfo();

        Assert.False(
            GuideAlgorithmContinuationLineParser.TryApply(
                "Bananas = 42",
                algorithm));
    }
}