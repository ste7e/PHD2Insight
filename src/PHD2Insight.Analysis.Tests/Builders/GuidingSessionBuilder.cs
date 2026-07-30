using PHD2Insight.Core.Models;
using Xunit.Sdk;

namespace PHD2Insight.Analysis.Tests.Builders;

internal sealed class GuidingSessionBuilder {
    private readonly List<GuideFrame> frames = [];
    private readonly List<SettlingEvent> settlingEvents = [];

    public GuidingSessionBuilder AddFrame(
        int frameNumber,
        TimeSpan elapsedTime,
        double raErrorArcSeconds = 0,
        double decErrorArcSeconds = 0,
        double? raPulseMilliseconds = 100,
        double? decPulseMilliseconds = 100) {
        frames.Add(new GuideFrame {
            FrameNumber = frameNumber,
            ElapsedTime = elapsedTime,

            RaErrorArcSeconds = raErrorArcSeconds,
            DecErrorArcSeconds = decErrorArcSeconds,

            // Default to guided frames
            RaPulseMilliseconds = raPulseMilliseconds,
            DecPulseMilliseconds = decPulseMilliseconds
        });

        return this;
    }

    public GuidingSessionBuilder AddFrame(
        double raErrorArcSeconds,
        double decErrorArcSeconds = 0,
        double? raPulseMilliseconds = 100,
        double? decPulseMilliseconds = 100) {
        return AddFrame(frames.Count + 1, TimeSpan.FromSeconds(frames.Count*2), raErrorArcSeconds, decErrorArcSeconds, raPulseMilliseconds, decPulseMilliseconds);
    }

    public GuidingSessionBuilder AddFrame(
        TimeSpan elapsedTime,
        double raErrorArcSeconds = 0,
        double decErrorArcSeconds = 0,
        double? raPulseMilliseconds = 100,
        double? decPulseMilliseconds = 100) {
        return AddFrame(frames.Count + 1, elapsedTime, raErrorArcSeconds, decErrorArcSeconds, raPulseMilliseconds, decPulseMilliseconds);
    }

    public GuidingSessionBuilder AddFrame(Action<GuideFrame> configure) {
        var frame = new GuideFrame();

        configure(frame);

        frames.Add(frame);

        return this;
    }

    public GuidingSessionBuilder AddFrame(GuideFrame frame) {
        frames.Add(frame);
        return this;
    }
    public GuidingSessionBuilder AddSettlingEvent(
        TimeSpan elapsedTime,
        SettlingState state) {
        settlingEvents.Add(new SettlingEvent {
            ElapsedTime = elapsedTime,
            State = state
        });

        return this;
    }

    public GuidingSession Build() {
        return new GuidingSession {
            Frames = frames,
            SettlingEvents = settlingEvents
        };
    }
}