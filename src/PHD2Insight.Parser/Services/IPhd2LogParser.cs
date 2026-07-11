using PHD2Insight.Core.Models;

namespace PHD2Insight.Parser.Services;


public interface IPhd2LogParser {
    GuidingSession Parse(string filename);
}
