using PHD2Insight.Core.Models;

namespace PHD2Insight.Parser.Internal;

internal sealed class GuideLogParseContext {
    public GuideLogVersion? Version { get; set; }

    public List<GuidingSession> Sessions { get; } = new();

    public GuidingSessionBuilder? CurrentSession { get; set; }

    public GuideLog Build() {
        var sessions = new List<GuidingSession>(Sessions);

        if (CurrentSession is not null) {
            sessions.Add(CurrentSession.Build());
        }

        return new GuideLog {
            Version = Version,
            Sessions = sessions
        };
    }

    public GuideAlgorithmInfo? CurrentXAlgorithm { get; set; }

    public GuideAlgorithmInfo? CurrentYAlgorithm { get; set; }
}