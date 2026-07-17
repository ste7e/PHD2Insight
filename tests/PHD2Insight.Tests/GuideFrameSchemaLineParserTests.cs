using PHD2Insight.Core.Models;
using PHD2Insight.Parser.Parsers;

namespace PHD2Insight.Tests.Parsers;

public sealed class GuideFrameSchemaLineParserTests {
    private const string ValidSchema =
        "Frame,Time,mount,dx,dy,RARawDistance,DECRawDistance,RAGuideDistance,DECGuideDistance,RADuration,RADirection,DECDuration,DECDirection,XStep,YStep,StarMass,SNR,ErrorCode";

    [Fact]
    public void Parses_valid_schema() {
        var result = GuideFrameSchemaLineParser.TryParse(
            ValidSchema,
            out var schema);

        Assert.True(result);

        Assert.NotNull(schema);

        Assert.Equal(18, schema.Columns.Count);

        Assert.Equal("Frame", schema.Columns.ElementAt(0));
        Assert.Equal("Time", schema.Columns.ElementAt(1));
        Assert.Equal("mount", schema.Columns.ElementAt(2));
        Assert.Equal("ErrorCode", schema.Columns.Last());
    }

    [Fact]
    public void Rejects_non_schema_line() {
        var result = GuideFrameSchemaLineParser.TryParse(
            "1,9.161,\"Mount\",1.189,-0.946",
            out var schema);

        Assert.False(result);
        Assert.Null(schema);
    }

    [Fact]
    public void Matches_expected_schema() {
        Assert.True(
            GuideFrameSchemaLineParser.TryParse(
                ValidSchema,
                out var schema));

        Assert.NotNull(schema);

        Assert.True(
            GuideFrameSchemaLineParser
                .MatchesExpectedSchema(schema));
    }

    [Fact]
    public void Detects_schema_mismatch() {
        const string ExtendedSchema =
            "Frame,Time,mount,dx,dy,RARawDistance,DECRawDistance,RAGuideDistance,DECGuideDistance,RADuration,RADirection,DECDuration,DECDirection,XStep,YStep,StarMass,SNR,ErrorCode,Temperature";

        Assert.True(
            GuideFrameSchemaLineParser.TryParse(
                ExtendedSchema,
                out var schema));

        Assert.NotNull(schema);

        Assert.False(
            GuideFrameSchemaLineParser
                .MatchesExpectedSchema(schema));
    }
}
