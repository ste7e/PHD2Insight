using PHD2Insight.Core.Models;
using PHD2Insight.Analysis.Diagnostics;

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
        return new GuidingSession {
            Frames =
            [
                new GuideFrame
            {
                FrameNumber = 339,
                ElapsedTime = TimeSpan.FromSeconds(3059.324)
            },
            new GuideFrame
            {
                FrameNumber = 340,
                ElapsedTime = TimeSpan.FromSeconds(3068.679)
            },
            new GuideFrame
            {
                FrameNumber = 341,
                ElapsedTime = TimeSpan.FromSeconds(3077.427)
            }
            ],

            SettlingEvents =
            [
                new SettlingEvent
            {
                State = SettlingState.Started,
                ElapsedTime = TimeSpan.FromSeconds(3059.500)
            },
            new SettlingEvent
            {
                State = SettlingState.Completed,
                ElapsedTime = TimeSpan.FromSeconds(3070.000)
            }
            ]
        };
    }
}