using PHD2Insight.Core.Models;
using PHD2Insight.Parser.Parsers;

namespace PHD2Insight.Tests.Parsers;

public sealed class GuideAlgorithmLineParserTests {
    [Fact]
    public void Parses_x_guide_algorithm() {
        const string line =
            "X guide algorithm = Predictive PEC, Control gain = 0.600";

        var result = GuideAlgorithmLineParser.TryParse(
            line,
            out var axis,
            out var algorithm);

        Assert.True(result);

        Assert.Equal(GuideAxis.X, axis);

        Assert.NotNull(algorithm);

        Assert.Equal(
            "Predictive PEC",
            algorithm.Name);

        Assert.Equal(
            0.600,
            algorithm.ControlGain);

        Assert.Null(algorithm.MinimumMove);
        Assert.Null(algorithm.Aggression);
        Assert.Null(algorithm.FastSwitch);
        Assert.Null(algorithm.PredictionGain);
    }

    [Fact]
    public void Parses_y_guide_algorithm() {
        const string line =
            "Y guide algorithm = Resist Switch, Minimum move = 0.300 Aggression = 70% FastSwitch = disabled";

        var result = GuideAlgorithmLineParser.TryParse(
            line,
            out var axis,
            out var algorithm);

        Assert.True(result);

        Assert.Equal(GuideAxis.Y, axis);

        Assert.NotNull(algorithm);

        Assert.Equal(
            "Resist Switch",
            algorithm.Name);

        Assert.Equal(
            0.300,
            algorithm.MinimumMove);

        Assert.Equal(
            70.0,
            algorithm.Aggression);

        Assert.False(
            algorithm.FastSwitch);

        Assert.Null(algorithm.ControlGain);
        Assert.Null(algorithm.PredictionGain);
    }
}