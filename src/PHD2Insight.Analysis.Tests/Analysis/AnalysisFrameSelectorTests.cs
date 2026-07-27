using PHD2Insight.Analysis.Diagnostics;
using PHD2Insight.Analysis.Tests.Builders;
using PHD2Insight.Core.Models;

namespace PHD2Insight.Analysis.Tests.Analysis;

public class AnalysisFrameSelectorTests {
    [Fact]
    public void Select_Excludes_Frames_During_Settling() {
        var session = CreateSession();

        var frames = AnalysisFrameSelector.GetAnalysisFrames(session);

        Assert.DoesNotContain(
            frames,
            f => f.FrameNumber == 340);

        Assert.Contains(
            frames,
            f => f.FrameNumber == 339);

        Assert.Contains(
            frames,
            f => f.FrameNumber == 341);
    }

    [Fact]
    public void GetAnalysisFrames_Excludes_Settling_Frame() {
        var session = CreateSession();

        var frames = AnalysisFrameSelector.GetAnalysisFrames(session);

        Assert.Collection(
            frames,
            frame => Assert.Equal(339, frame.FrameNumber),
            frame => Assert.Equal(341, frame.FrameNumber));
    }

    private static GuidingSession CreateSession() {
        var builder = new GuidingSessionBuilder();

        builder.AddFrame(339, TimeSpan.FromSeconds(3059.324), raErrorArcSeconds: 1.0);
        builder.AddFrame(340, TimeSpan.FromSeconds(3068.679), raErrorArcSeconds: -1.0);
        builder.AddFrame(341, TimeSpan.FromSeconds(3077.427), raErrorArcSeconds: 1.0);

        builder.AddSettlingEvent(TimeSpan.FromSeconds(3059.500), SettlingState.Started);
        builder.AddSettlingEvent(TimeSpan.FromSeconds(3070.000), SettlingState.Completed);

        return builder.Build();
    }
}