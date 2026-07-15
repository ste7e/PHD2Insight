using Xunit;

namespace PHD2Insight.Tests;

public class SampleLogTests {
    [Fact]
    public void SampleLogExists() {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "Fixtures",
            "PHD2_GuideLog_001");

        Assert.True(File.Exists(path));

        var lines = File.ReadAllLines(path);

        Assert.NotEmpty(lines);
    }
}