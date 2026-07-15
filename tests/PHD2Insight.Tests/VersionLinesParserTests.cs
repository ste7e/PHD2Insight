using PHD2Insight.Parser.Parsers;
using Xunit;

namespace PHD2Insight.Tests;

public sealed class VersionLineParserTests {
    [Theory]
    [InlineData(
        "PHD2 version 2.6.14 [Windows], Log version 2.5.",
        "2.6.14 [Windows]",
        "2.5")]

    [InlineData(
        "PHD2 version 2.6.14 [Windows], Log version 2.5",
        "2.6.14 [Windows]",
        "2.5")]
    public void Parses_version_line(
        string line,
        string expectedPhd2,
        string expectedLog) {
        var result = VersionLineParser.TryParse(
            line,
            out var version);

        Assert.True(result);
        Assert.NotNull(version);

        Assert.Equal(expectedPhd2, version.Phd2Version);
        Assert.Equal(expectedLog, version.LogVersion);
    }
}