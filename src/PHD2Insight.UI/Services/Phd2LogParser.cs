using System.Globalization;
using PHD2Insight.UI.Models;


namespace PHD2Insight.UI.Services;


public class Phd2LogParser {

    public GuidingSession Parse(string filename) {
        var session = new GuidingSession();


        foreach (var line in File.ReadLines(filename)) {

            if (!line.Contains("GUIDE"))
                continue;


            var parts =
                line.Split(',');


            if (parts.Length < 5)
                continue;


            if (!double.TryParse(
                parts[2],
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out var ra))
                continue;


            if (!double.TryParse(
                parts[3],
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out var dec))
                continue;


            session.Frames.Add(
                new GuideFrame {
                    Timestamp = DateTime.Now,
                    RaError = ra,
                    DecError = dec
                });
        }


        return session;
    }

}