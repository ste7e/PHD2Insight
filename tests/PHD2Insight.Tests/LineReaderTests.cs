using System.Text;
using PHD2Insight.Parser.Infrastructure;
using Xunit;

namespace PHD2Insight.Tests;

public sealed class LineReaderTests {
    [Fact]
    public void Read_advances_line_number() {
        const string text =
"""
One
Two
Three
""";

        using var stream =
            new MemoryStream(Encoding.UTF8.GetBytes(text));

        using var reader = new LineReader(stream);

        Assert.True(reader.Read());
        Assert.Equal(1, reader.LineNumber);
        Assert.Equal("One", reader.CurrentLine);

        Assert.True(reader.Read());
        Assert.Equal(2, reader.LineNumber);
        Assert.Equal("Two", reader.CurrentLine);

        Assert.True(reader.Read());
        Assert.Equal(3, reader.LineNumber);
        Assert.Equal("Three", reader.CurrentLine);

        Assert.False(reader.Read());
    }
}