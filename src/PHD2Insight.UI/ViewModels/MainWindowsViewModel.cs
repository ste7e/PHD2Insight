using CommunityToolkit.Mvvm.ComponentModel;
using PHD2Insight.UI.Services;
using PHD2Insight.UI.Utilities;


namespace PHD2Insight.UI.ViewModels;


public class MainWindowViewModel :
    ObservableObject {

    private readonly PHD2LogAnalysisService analysisService;


    private string status = "No log loaded";

    public string Status {
        get => status;
        private set => SetProperty(ref status, value);
    }


    private string metrics = "";

    public string Metrics {
        get => metrics;
        private set => SetProperty(ref metrics, value);
    }



    public MainWindowViewModel() {
        analysisService =
            new PHD2LogAnalysisService();

    }


    public void LoadLog(string filePath) {

        try {
            Status = "Analysing log...";
            Metrics = "";

            var result =
                analysisService.Analyse(filePath);

            Status =
                $"{result.FileName} — " +
                $"{result.Sessions.Count} session(s)";

            Metrics =
                BuildSessionSummary(result);
        } catch (Exception ex) {
            Status = "Unable to analyse log";
            Metrics = ex.Message;
        }
    }


    
    private static string BuildSessionSummary(
        LogAnalysisResult result) {

        var lines = new List<string>();

        foreach (var session in result.Sessions) {

            lines.Add(
                $"Session {session.SessionNumber + 1}");

            if (session.Diagnoses.Count == 0) {
                lines.Add("  No significant diagnosis");
            } else {
                foreach (var diagnosis in session.Diagnoses) {
                    lines.Add(
                        $"  {diagnosis.Title} " +
                        $"({diagnosis.Score}, {diagnosis.Confidence})");
                }
            }

            lines.Add("");
        }

        return string.Join(
            Environment.NewLine,
            lines);
    }
}