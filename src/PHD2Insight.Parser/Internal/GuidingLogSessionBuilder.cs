using PHD2Insight.Core.Models;

namespace PHD2Insight.Parser.Internal;

internal sealed class GuidingSessionBuilder {
    public DateTime StartTime { get; init; }

    public GuidingSession Build() {
        return new GuidingSession {
            StartTime = StartTime
        };
    }
}