using PHD2Insight.Analysis.Analysis;
using PHD2Insight.Core.Models;

namespace PHD2Insight.Analysis.Tests.Analysis;

public sealed class SettlingAnalysisTests {
    [Fact]
    public void Calculate_Returns_Empty_Result_For_No_Events() {
        var session = new GuidingSession();

        var result = SettlingAnalysis.Calculate(session);

        Assert.Equal(0, result.SettlingAttemptCount);
        Assert.Equal(0, result.SuccessfulSettles);
        Assert.Equal(0, result.FailedSettles);
        Assert.Equal(0, result.CancelledSettles);

        Assert.Equal(TimeSpan.Zero, result.AverageSettlingTime);
        Assert.Equal(TimeSpan.Zero, result.ShortestSettlingTime);
        Assert.Equal(TimeSpan.Zero, result.LongestSettlingTime);
    }

    [Fact]
    public void Calculate_Returns_Successful_Settling_Statistics() {
        var session = new GuidingSession {
            SettlingEvents =
            [
                new SettlingEvent
                {
                    ElapsedTime = TimeSpan.FromSeconds(10),
                    State = SettlingState.Started
                },
                new SettlingEvent
                {
                    ElapsedTime = TimeSpan.FromSeconds(16),
                    State = SettlingState.Completed
                },
                new SettlingEvent
                {
                    ElapsedTime = TimeSpan.FromSeconds(30),
                    State = SettlingState.Started
                },
                new SettlingEvent
                {
                    ElapsedTime = TimeSpan.FromSeconds(35),
                    State = SettlingState.Completed
                }
            ]
        };

        var result = SettlingAnalysis.Calculate(session);

        Assert.Equal(2, result.SettlingAttemptCount);
        Assert.Equal(2, result.SuccessfulSettles);

        Assert.Equal(
            TimeSpan.FromSeconds(5.5),
            result.AverageSettlingTime);

        Assert.Equal(
            TimeSpan.FromSeconds(5),
            result.ShortestSettlingTime);

        Assert.Equal(
            TimeSpan.FromSeconds(6),
            result.LongestSettlingTime);
    }

    [Fact]
    public void Calculate_Counts_Failed_And_Cancelled_Settles() {
        var session = new GuidingSession {
            SettlingEvents =
            [
                new SettlingEvent
                {
                    ElapsedTime = TimeSpan.FromSeconds(10),
                    State = SettlingState.Started
                },
                new SettlingEvent
                {
                    ElapsedTime = TimeSpan.FromSeconds(15),
                    State = SettlingState.Failed
                },
                new SettlingEvent
                {
                    ElapsedTime = TimeSpan.FromSeconds(20),
                    State = SettlingState.Started
                },
                new SettlingEvent
                {
                    ElapsedTime = TimeSpan.FromSeconds(22),
                    State = SettlingState.Cancelled
                }
            ]
        };

        var result = SettlingAnalysis.Calculate(session);

        Assert.Equal(2, result.SettlingAttemptCount);

        Assert.Equal(0, result.SuccessfulSettles);
        Assert.Equal(1, result.FailedSettles);
        Assert.Equal(1, result.CancelledSettles);

        Assert.Equal(TimeSpan.Zero, result.AverageSettlingTime);
    }

    [Fact]
    public void Calculate_Ignores_Unmatched_Completed_Event() {
        var session = new GuidingSession {
            SettlingEvents =
            [
                new SettlingEvent
                {
                    ElapsedTime = TimeSpan.FromSeconds(5),
                    State = SettlingState.Completed
                }
            ]
        };

        var result = SettlingAnalysis.Calculate(session);

        Assert.Equal(0, result.SuccessfulSettles);
        Assert.Equal(0, result.SettlingAttemptCount);
    }
}