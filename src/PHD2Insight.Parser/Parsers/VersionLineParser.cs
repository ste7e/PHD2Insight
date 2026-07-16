using PHD2Insight.Core.Models;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace PHD2Insight.Parser.Parsers;

internal static partial class VersionLineParser {
    [GeneratedRegex(
        @"PHD2 version (?<phd2>[^,]+), Log version (?<log>[0-9]+(?:\.[0-9]+)*)\.?",
        RegexOptions.Compiled)]
    private static partial Regex VersionRegex();

    public static bool TryParse(
        string line,
        [NotNullWhen(true)] out GuideLogVersion? version) {
        version = null;

        var match = VersionRegex().Match(line);

        if (!match.Success)
            return false;

        version = new GuideLogVersion {
            Phd2Version = match.Groups["phd2"].Value,
            LogVersion = match.Groups["log"].Value
        };

        return true;
    }
}