using PHD2Insight.Core.Models;
using PHD2Insight.Parser.Parsers;

namespace PHD2Insight.Parser.Tests.Parser;

public sealed class SettlingStateParserTests {
    [Theory]
    [InlineData("Settling started", SettlingState.Started)]
    [InlineData("Settling complete", SettlingState.Completed)]
    [InlineData("Settling failed", SettlingState.Failed)]
    [InlineData("Settling cancelled", SettlingState.Cancelled)]
    [InlineData("Something unexpected", SettlingState.Unknown)]
    public void Parse_Returns_Expected_State(
        string input,
        SettlingState expected) {
        var result = SettlingStateParser.Parse(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Parse_Throws_For_Null() {
        Assert.Throws<ArgumentNullException>(
            () => SettlingStateParser.Parse(null!));
    }
}