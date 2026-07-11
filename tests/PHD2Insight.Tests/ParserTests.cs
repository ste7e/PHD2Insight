using PHD2Insight.Parser.Services;


namespace PHD2Insight.Tests;


public class ParserTests {

    [Fact]
    public void EmptyLogProducesEmptySession() {
        var filename =
            Path.GetTempFileName();


        try {
            var parser =
                new Phd2LogParser();


            var result =
                parser.Parse(filename);


            Assert.NotNull(result);

            Assert.Empty(result.Samples);
        } finally {
            File.Delete(filename);
        }
    }

}