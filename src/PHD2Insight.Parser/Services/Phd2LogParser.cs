using PHD2Insight.Core.Models;


namespace PHD2Insight.Parser.Services;


public sealed class Phd2LogParser : IPhd2LogParser {

    public GuidingSession Parse(string filename) {
        var session = new GuidingSession();


        foreach (var line in File.ReadLines(filename)) {
            ParseLine(line, session);
        }


        return session;
    }


    private static void ParseLine(
        string line,
        GuidingSession session) {
        /*
         * Real PHD2 parsing starts here.
         *
         * This first version deliberately does nothing.
         * We will add the actual grammar after we lock
         * down the log formats we support.
         */
    }

}