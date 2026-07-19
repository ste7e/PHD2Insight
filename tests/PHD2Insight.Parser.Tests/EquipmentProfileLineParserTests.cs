using PHD2Insight.Parser.Parsers;
using Xunit;

namespace PHD2Insight.Tests;

public sealed class EquipmentProfileLineParserTests {
    [Fact]
    public void Parses_equipment_profile() {
        const string line =
            "Equipment Profile = QHY5 EQASCOM OAG";

        var result = EquipmentProfileLineParser.TryParse(
            line,
            out var profile);

        Assert.True(result);
        Assert.Equal("QHY5 EQASCOM OAG", profile);
    }

    [Fact]
    public void Rejects_non_equipment_profile_line() {
        var result = EquipmentProfileLineParser.TryParse(
            "Guiding Begins at 2026-07-09 22:54:58",
            out var profile);

        Assert.False(result);
        Assert.Null(profile);
    }
}