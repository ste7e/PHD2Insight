using System.Diagnostics.CodeAnalysis;
using PHD2Insight.Core.Models;

namespace PHD2Insight.Parser.Parsers;

internal static class GuideFrameSchemaLineParser {
    private static readonly string[] ExpectedColumns =
    {
        "Frame",
        "Time",
        "mount",
        "dx",
        "dy",
        "RARawDistance",
        "DECRawDistance",
        "RAGuideDistance",
        "DECGuideDistance",
        "RADuration",
        "RADirection",
        "DECDuration",
        "DECDirection",
        "XStep",
        "YStep",
        "StarMass",
        "SNR",
        "ErrorCode"
    };

    public static bool TryParse(
        string line,
        [NotNullWhen(true)] out GuideFrameSchema? schema) {
        schema = null;

        if (!line.StartsWith(
                "Frame,",
                StringComparison.Ordinal)) {
            return false;
        }

        var columns = line.Split(',');

        schema = new GuideFrameSchema();

        foreach (var column in columns) {
            schema.Columns.Add(column.Trim());
        }

        return true;
    }

    public static bool MatchesExpectedSchema(
        GuideFrameSchema schema) {
        if (schema.Columns.Count != ExpectedColumns.Length) {
            return false;
        }

        return schema.Columns.SequenceEqual(
            ExpectedColumns,
            StringComparer.Ordinal);
    }
}