using PHD2Insight.Parser.Parsers;

namespace PHD2Insight.Tests.Parsers;

public sealed class CsvLineParserTests {
    [Fact]
    public void Parses_simple_csv() {
        var fields = CsvLineParser.Parse("A,B,C");

        Assert.Equal(3, fields.Count);
        Assert.Equal("A", fields[0]);
        Assert.Equal("B", fields[1]);
        Assert.Equal("C", fields[2]);
    }

    [Fact]
    public void Parses_quoted_field() {
        var fields = CsvLineParser.Parse(
            "1,2,\"Mount\",4");

        Assert.Equal(4, fields.Count);
        Assert.Equal("Mount", fields[2]);
    }

    [Fact]
    public void Parses_empty_fields() {
        var fields = CsvLineParser.Parse(
            "A,,C");

        Assert.Equal(3, fields.Count);
        Assert.Equal(string.Empty, fields[1]);
    }

    [Fact]
    public void Parses_trailing_empty_field() {
        var fields = CsvLineParser.Parse(
            "A,B,");

        Assert.Equal(3, fields.Count);
        Assert.Equal(string.Empty, fields[2]);
    }

    [Fact]
    public void Parses_quoted_field_containing_comma() {
        var fields = CsvLineParser.Parse(
            "1,\"Star lost, low mass\",3");

        Assert.Equal(3, fields.Count);
        Assert.Equal("Star lost, low mass", fields[1]);
    }

    [Fact]
    public void Parses_drop_record() {
        var fields = CsvLineParser.Parse(
            "710,5341.627,\"DROP\",,,,,,,,,,,,,0,0.00,3,\"Star lost - low mass\"");

        Assert.Equal("710", fields[0]);
        Assert.Equal("5341.627", fields[1]);
        Assert.Equal("DROP", fields[2]);
        Assert.Equal("0", fields[15]);
        Assert.Equal("0.00", fields[16]);
        Assert.Equal("3", fields[17]);
        Assert.Equal("Star lost - low mass", fields[18]);
    }
}