using PHD2Insight.Core.Models;

namespace PHD2Insight.Parser.Internal;

internal sealed class GuideLogParseContext {
    public GuideLogVersion? Version { get; set; }

    public List<GuidingSession> Sessions { get; } = new();

    public GuidingSessionBuilder? CurrentSession { get; set; }

    public GuideLog Build() {
        return new GuideLog {
            Version = Version,
            Sessions = Sessions
        };
    }
}