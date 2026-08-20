using PHD2Insight.Analysis.Diagnostics;
using PHD2Insight.Analysis.Models;
using PHD2Insight.Analysis.Observations;
using PHD2Insight.Analysis.Recommendations;
using PHD2Insight.UI.Services;

namespace PHD2Insight.UI.Tests.Services;

public sealed class PHD2LogAnalysisServiceTests {

    private static readonly string SampleFolder =
        Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "..",
                "..",
                "..",
                "..",
                "..",
                "samples"));

    private static string SamplePath =>
        Path.Combine(
            SampleFolder,
            "PHD2_GuideLog_001.txt");


    [Fact]
    public void Analyse_ReturnsExpectedSessions() {
        // Arrange
        var service = new PHD2LogAnalysisService();

        // Act
        var result = service.Analyse(SamplePath);

        // Assert
        Assert.Equal(
            "PHD2_GuideLog_001.txt",
            result.FileName);

        Assert.Equal(
            4,
            result.Sessions.Count);

        Assert.All(
            result.Sessions,
            session => Assert.NotNull(session.Analysis));
    }


    [Fact]
    public void Analyse_AssignsSessionNumbers() {
        // Arrange
        var service = new PHD2LogAnalysisService();

        // Act
        var result = service.Analyse(SamplePath);

        // Assert
        Assert.Equal(
            new[] { 0, 1, 2, 3 },
            result.Sessions
                .Select(s => s.SessionNumber));
    }


    [Fact]
    public void Analyse_PopulatesDiagnoses() {
        // Arrange
        var service = new PHD2LogAnalysisService();

        // Act
        var result = service.Analyse(SamplePath);

        // Assert
        Assert.Contains(
            result.Sessions,
            session => session.Diagnoses.Count > 0);
    }


    [Fact]
    public void Analyse_PopulatesRecommendations() {
        // Arrange
        var service = new PHD2LogAnalysisService();

        // Act
        var result = service.Analyse(SamplePath);

        // Assert
        var sessionsWithRecommendations = result.Sessions
            .Where(s => s.Recommendations.Count > 0)
            .ToList();

        Assert.NotEmpty(sessionsWithRecommendations);

        Assert.Contains(
            sessionsWithRecommendations.SelectMany(
                s => s.Recommendations),
            recommendation =>
                recommendation.Code ==
                "REVIEW_RA_GUIDING_AGGRESSIVENESS");
    }

    [Fact]
    public void Analyse_RecommendationIsSupportedByDiagnosis() {
        // Arrange
        var service = new PHD2LogAnalysisService();

        // Act
        var result = service.Analyse(SamplePath);

        // Assert
        var sessionsWithRaOscillation = result.Sessions
            .Where(s => s.Diagnoses.Any(
                d => d.Code == "RA_OSCILLATION"))
            .ToList();

        Assert.NotEmpty(sessionsWithRaOscillation);

        foreach (var session in sessionsWithRaOscillation) {
            var recommendation = Assert.Single(session.Recommendations, r => r.Code ==
                        "REVIEW_RA_GUIDING_AGGRESSIVENESS");

            Assert.Contains(
                "RA_OSCILLATION",
                recommendation.SupportingDiagnosisCodes);

            Assert.Contains(
                session.Diagnoses,
                diagnosis =>
                    diagnosis.Code == "RA_OSCILLATION");
        }
    }

    [Fact]
    public void Analyse_MissingFile_Throws() {
        // Arrange
        var service = new PHD2LogAnalysisService();

        var path = Path.Combine(
            SampleFolder,
            "does-not-exist.txt");

        // Act & Assert
        Assert.Throws<FileNotFoundException>(
            () => service.Analyse(path));
    }
}