using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PHD2Insight.Analysis.Models;
using PHD2Insight.UI.Services;
using System.Collections.ObjectModel;

namespace PHD2Insight.UI.ViewModels;

public partial class LogViewModel : ViewModelBase {

    private readonly ObservableCollection<SessionViewModel> allSessions;


    public LogViewModel(
        LogAnalysisResult result) {

        FileName = result.FileName;

        allSessions =
            new ObservableCollection<SessionViewModel>(
                result.Sessions.Select(
                    session => new SessionViewModel(
                        session.SessionNumber,
                        session.Session,
                        session.Analysis,
                        session.Diagnoses,
                        session.Quality)));

        Sessions =
            new ObservableCollection<SessionViewModel>(
                allSessions);

        ToggleExpansionCommand =
            new RelayCommand(
                () => IsExpanded = !IsExpanded);

        SortSessionsCommand =
            new RelayCommand<string?>(
                SortSessions);
    }


    public string FileName {
        get;
    }

    public GuidingQuality Quality =>
    Sessions
        .Select(s => s.Quality)
        .DefaultIfEmpty(GuidingQuality.Unknown)
        .Max();

    public string QualityDisplay =>
    Quality switch {
        GuidingQuality.Good => "Good",
        GuidingQuality.Acceptable => "Acceptable",
        GuidingQuality.Poor => "Poor",
        _ => "Unknown"
    };


    public ObservableCollection<SessionViewModel> Sessions {
        get;
    }


    public int SessionCount =>
        Sessions.Count;


    public DateTime? LogDateTime =>
        Sessions
            .Select(s => s.Session.StartTime)
            .OrderBy(t => t)
            .FirstOrDefault();


    public double AverageTotalRms =>
        Sessions
            .Select(s => s.TotalRmsValue)
            .Where(v => v.HasValue)
            .Select(v => v!.Value)
            .DefaultIfEmpty()
            .Average();


    public double BestTotalRms =>
        Sessions
            .Select(s => s.TotalRmsValue)
            .Where(v => v.HasValue)
            .Select(v => v!.Value)
            .DefaultIfEmpty()
            .Min();


    public double WorstTotalRms =>
        Sessions
            .Select(s => s.TotalRmsValue)
            .Where(v => v.HasValue)
            .Select(v => v!.Value)
            .DefaultIfEmpty()
            .Max();


    public int DiagnosisCount =>
        Sessions.Sum(
            s => s.Diagnoses.Count);


    public string DisplayDateTime =>
        LogDateTime.HasValue
            ? LogDateTime.Value.ToString("dd MMM yyyy HH:mm")
            : "—";


    public string AverageTotalRmsDisplay =>
        AverageTotalRms > 0
            ? $"{AverageTotalRms:F2}\""
            : "—";


    public string BestTotalRmsDisplay =>
        BestTotalRms > 0
            ? $"{BestTotalRms:F2}\""
            : "—";


    public string WorstTotalRmsDisplay =>
        WorstTotalRms > 0
            ? $"{WorstTotalRms:F2}\""
            : "—";


    public string DiagnosisCountDisplay =>
        DiagnosisCount == 1
            ? "1"
            : $"{DiagnosisCount}";


    public IRelayCommand ToggleExpansionCommand {
        get;
    }


    public IRelayCommand<string?> SortSessionsCommand {
        get;
    }

    public string SessionHeading =>
    GetSessionSortIndicator("Session", "Session");

    public string StartTimeHeading =>
        GetSessionSortIndicator("StartTime", "Start");

    public string DurationHeading =>
        GetSessionSortIndicator("Duration", "Duration");

    public string RaRmsHeading =>
        GetSessionSortIndicator("RaRms", "RA RMS");

    public string DecRmsHeading =>
        GetSessionSortIndicator("DecRms", "DEC RMS");

    public string TotalRmsHeading =>
        GetSessionSortIndicator("TotalRms", "Total RMS");

    public string DiagnosesHeading =>
        GetSessionSortIndicator("Diagnoses", "Diagnoses");

    public string QualityHeading =>
        GetSessionSortIndicator("Quality", "Quality");

    [ObservableProperty]
    private bool isExpanded;


    private string sessionSortColumn =
        "Session";


    private SortDirection sessionSortDirection =
        SortDirection.Ascending;


    private void SortSessions(
        string? column) {

        if (string.IsNullOrWhiteSpace(column))
            return;


        if (column == sessionSortColumn) {

            sessionSortDirection =
                sessionSortDirection ==
                    SortDirection.Ascending
                        ? SortDirection.Descending
                        : SortDirection.Ascending;

        } else {

            sessionSortColumn = column;
            sessionSortDirection =
                SortDirection.Ascending;
        }


        IEnumerable<SessionViewModel> sorted =
            column switch {

                "Session" =>
                    sessionSortDirection ==
                        SortDirection.Ascending
                            ? allSessions.OrderBy(
                                s => s.SessionNumber)
                            : allSessions.OrderByDescending(
                                s => s.SessionNumber),

                "StartTime" =>
                    sessionSortDirection ==
                        SortDirection.Ascending
                            ? allSessions.OrderBy(
                                s => s.Session.StartTime)
                            : allSessions.OrderByDescending(
                                s => s.Session.StartTime),

                "Duration" =>
                    sessionSortDirection ==
                        SortDirection.Ascending
                            ? allSessions.OrderBy(
                                GetDuration)
                            : allSessions.OrderByDescending(
                                GetDuration),

                "RaRms" =>
                    sessionSortDirection ==
                        SortDirection.Ascending
                            ? allSessions.OrderBy(
                                s => s.Analysis.Rms?.RaArcSeconds
                                    ?? double.MaxValue)
                            : allSessions.OrderByDescending(
                                s => s.Analysis.Rms?.RaArcSeconds
                                    ?? double.MinValue),

                "DecRms" =>
                    sessionSortDirection ==
                        SortDirection.Ascending
                            ? allSessions.OrderBy(
                                s => s.Analysis.Rms?.DecArcSeconds
                                    ?? double.MaxValue)
                            : allSessions.OrderByDescending(
                                s => s.Analysis.Rms?.DecArcSeconds
                                    ?? double.MinValue),

                "TotalRms" =>
                    sessionSortDirection ==
                        SortDirection.Ascending
                            ? allSessions.OrderBy(
                                s => s.TotalRmsValue
                                    ?? double.MaxValue)
                            : allSessions.OrderByDescending(
                                s => s.TotalRmsValue
                                    ?? double.MinValue),

                "Diagnoses" =>
                    sessionSortDirection ==
                        SortDirection.Ascending
                            ? allSessions.OrderBy(
                                s => s.Diagnoses.Count)
                            : allSessions.OrderByDescending(
                                s => s.Diagnoses.Count),

                "Quality" =>
                    sessionSortDirection ==
                    SortDirection.Ascending
                    ? allSessions.OrderBy(
                    s => s.Quality)
                    : allSessions.OrderByDescending(
                    s => s.Quality),

                _ =>
                    allSessions
            };


        Sessions.Clear();

        foreach (var session in sorted)
            Sessions.Add(session);

        OnPropertyChanged(nameof(SessionHeading));
        OnPropertyChanged(nameof(StartTimeHeading));
        OnPropertyChanged(nameof(DurationHeading));
        OnPropertyChanged(nameof(RaRmsHeading));
        OnPropertyChanged(nameof(DecRmsHeading));
        OnPropertyChanged(nameof(TotalRmsHeading));
        OnPropertyChanged(nameof(DiagnosesHeading));
        OnPropertyChanged(nameof(QualityHeading));
    }

    private string GetSessionSortIndicator(string column, string heading) {
        if (sessionSortColumn != column)
            return heading;

        return sessionSortDirection == SortDirection.Ascending
            ? $"{heading} ▲"
            : $"{heading} ▼";
    }
    private static double GetDuration(
        SessionViewModel session) {

        if (session.Session.EndTime is null)
            return double.MaxValue;

        return (
            session.Session.EndTime.Value -
            session.Session.StartTime)
            .TotalSeconds;
    }
}