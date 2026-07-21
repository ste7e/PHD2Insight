using PHD2Insight.Core.Models;
using PHD2Insight.Parser.Parsers;

namespace PHD2Insight.Parser.Tests.Parser;

public sealed class SettlingEventParserTests {
    [Fact]
    public void TryParse_Returns_True_For_Settling_Line() {
        const string line =
            "INFO: SETTLING STATE CHANGE, Settling started";

        var result = SettlingEventParser.TryParse(
            line,
            out var settlingEvent);

        Assert.True(result);
        Assert.NotNull(settlingEvent);
        Assert.Equal(SettlingState.Started, settlingEvent.State);
    }

    [Fact]
    public void TryParse_Returns_False_For_Non_Settling_Line() {
        const string line =
            "1,9.161,\"Mount\",1.189,-0.946,...";

        var result = SettlingEventParser.TryParse(
            line,
            out var settlingEvent);

        Assert.False(result);
        Assert.Null(settlingEvent);
    }
}