using PHD2Insight.Analysis.Analysis;
using PHD2Insight.Core.Models;

namespace PHD2Insight.Analysis.Tests.Analysis;

public sealed class LostStarAnalysisTests {
    [Fact]
    public void Calculate_Returns_Zero_For_Session_Without_Lost_Stars() {
        var session = new GuidingSession {
            Frames =
            [
                new GuideFrame
                {
                    ElapsedTime = TimeSpan.FromSeconds(1),
                    ErrorCode = 0
                },
                new GuideFrame
                {
                    ElapsedTime = TimeSpan.FromSeconds(2),
                    ErrorCode = 0
                }
            ]
        };

        var result = LostStarAnalysis.Calculate(session);

        Assert.Equal(0, result.LostStarCount);
        Assert.Equal(0.0, result.LostStarPercentage);
        Assert.Null(result.FirstOccurrence);
        Assert.Null(result.LastOccurrence);
        Assert.Empty(result.ErrorMessages);
    }

    [Fact]
    public void Calculate_Returns_Lost_Star_Statistics() {
        var session = new GuidingSession {
            Frames =
            [
                new GuideFrame
                {
                    ElapsedTime = TimeSpan.FromSeconds(1)
                },
                new GuideFrame
                {
                    ElapsedTime = TimeSpan.FromSeconds(5),
                    ErrorCode = 3,
                    ErrorDescription = "Star lost - low mass"
                },
                new GuideFrame
                {
                    ElapsedTime = TimeSpan.FromSeconds(9),
                    ErrorCode = 3,
                    ErrorDescription = "Star lost - low mass"
                },
                new GuideFrame
                {
                    ElapsedTime = TimeSpan.FromSeconds(12),
                    ErrorCode = 7,
                    ErrorDescription = "Star saturated"
                }
            ]
        };

        var result = LostStarAnalysis.Calculate(session);

        Assert.Equal(3, result.LostStarCount);

        Assert.Equal(75.0, result.LostStarPercentage);

        Assert.Equal(
            TimeSpan.FromSeconds(5),
            result.FirstOccurrence);

        Assert.Equal(
            TimeSpan.FromSeconds(12),
            result.LastOccurrence);

        Assert.Equal(2, result.ErrorMessages.Count);

        Assert.Equal(
            2,
            result.ErrorMessages["Star lost - low mass"]);

        Assert.Equal(
            1,
            result.ErrorMessages["Star saturated"]);
    }

    [Fact]
    public void Calculate_Groups_Unknown_Error_Messages() {
        var session = new GuidingSession {
            Frames =
            [
                new GuideFrame
                {
                    ElapsedTime = TimeSpan.FromSeconds(1),
                    ErrorCode = 99
                }
            ]
        };

        var result = LostStarAnalysis.Calculate(session);

        Assert.Single(result.ErrorMessages);

        Assert.Equal(
            1,
            result.ErrorMessages["Unknown"]);
    }
}