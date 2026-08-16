using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PHD2Insight.UI.Services;

namespace PHD2Insight.UI.ViewModels;

public partial class ExplorerViewModel : ViewModelBase {

    private readonly PHD2LogAnalysisService analysisService;


    public ExplorerViewModel() {

        analysisService = new PHD2LogAnalysisService();

        settings = new ExplorerSettings();

        OpenFolderCommand =
            new AsyncRelayCommand(
                async () => {
                    var path =
                        await RequestFolderAsync?.Invoke()
                        ?? null;

                    if (!string.IsNullOrWhiteSpace(path))
                        LoadFolder(path);
                });

        SortLogsCommand =
            new RelayCommand<string?>(
                SortLogs);

        RestoreLastFolder();
    }

    private void RestoreLastFolder() {

        var path = settings.LastFolder;

        if (string.IsNullOrWhiteSpace(path))
            return;

        if (!Directory.Exists(path))
            return;

        LoadFolder(path);
    }
    public Func<Task<string?>>? RequestFolderAsync { get; set; }


    public IAsyncRelayCommand OpenFolderCommand { get; }


    public ObservableCollection<FolderViewModel> Folders { get; } = [];


    [ObservableProperty]
    private FolderViewModel? selectedFolder;


    [ObservableProperty]
    private LogViewModel? selectedLog;


    [ObservableProperty]
    private SessionViewModel? selectedSession;

    public IRelayCommand<string?> SortLogsCommand {
        get;
    }

    private readonly ExplorerSettings settings;

    private string logSortColumn =
    "Date";

    private string GetLogSortIndicator(string column) =>
    logSortColumn == column
        ? logSortDirection == SortDirection.Ascending
            ? " ▲"
            : " ▼"
        : string.Empty;

    public string DateHeading => $"Log / Date{GetLogSortIndicator("Date")}";
    public string SessionsHeading => $"Sessions{GetLogSortIndicator("Sessions")}";
    public string AverageRmsHeading => $"Avg RMS{GetLogSortIndicator("AverageRms")}";
    public string BestRmsHeading => $"Best RMS{GetLogSortIndicator("BestRms")}";
    public string WorstRmsHeading => $"Worst RMS{GetLogSortIndicator("WorstRms")}";
    public string DiagnosesHeading => $"Diagnoses{GetLogSortIndicator("Diagnoses")}";
    public string QualityHeading => $"Quality{GetLogSortIndicator("Quality")}";

    private SortDirection logSortDirection = SortDirection.Descending;
    public void LoadFolder(string path) {

        if (!Directory.Exists(path))
            return;

        settings.SaveLastFolder(path);

        Folders.Clear();

        var root =
            new FolderViewModel(
                Path.GetFileName(
                    Path.TrimEndingDirectorySeparator(path)),
                path);

        LoadLogs(root);

        Folders.Add(root);

        foreach (var directory in Directory
            .EnumerateDirectories(path)
            .OrderBy(Path.GetFileName)) {

            Folders.Add(
                new FolderViewModel(
                    Path.GetFileName(directory),
                    directory));
        }

        SelectedFolder = root;
    }
    private void LoadLogs(
        FolderViewModel folder) {

        folder.Logs.Clear();

        foreach (var file in Directory
            .EnumerateFiles(
                folder.Path,
                "PHD2_GuideLog_*.txt",
                SearchOption.TopDirectoryOnly)
            .OrderByDescending(File.GetLastWriteTime)) {

            try {
                var result =
                    analysisService.Analyse(file);

                if (result.Sessions.Count == 0)
                    continue;

                folder.Logs.Add(
                    new LogViewModel(result));

            } catch {
                // We'll report unreadable logs properly later.
            }
        }

        SortLogs(logSortColumn);
    }
    partial void OnSelectedFolderChanged(
    FolderViewModel? value) {

        if (value is null)
            return;

        LoadLogs(value);
    }
    private void SortLogs(
    string? column) {

        if (string.IsNullOrWhiteSpace(column))
            return;


        if (column == logSortColumn) {

            logSortDirection =
                logSortDirection ==
                    SortDirection.Ascending
                        ? SortDirection.Descending
                        : SortDirection.Ascending;

        } else {

            logSortColumn = column;

            logSortDirection =
                column == "Date"
                    ? SortDirection.Descending
                    : SortDirection.Ascending;
        }


        if (SelectedFolder is null)
            return;


        var logs =
            SelectedFolder.Logs.ToList();


        IEnumerable<LogViewModel> sorted =
            column switch {

                "Date" =>
                    logSortDirection ==
                        SortDirection.Ascending
                            ? logs.OrderBy(
                                l => l.LogDateTime)
                            : logs.OrderByDescending(
                                l => l.LogDateTime),

                "FileName" =>
                    logSortDirection ==
                        SortDirection.Ascending
                            ? logs.OrderBy(
                                l => l.FileName)
                            : logs.OrderByDescending(
                                l => l.FileName),

                "Sessions" =>
                    logSortDirection ==
                        SortDirection.Ascending
                            ? logs.OrderBy(
                                l => l.SessionCount)
                            : logs.OrderByDescending(
                                l => l.SessionCount),

                "AverageRms" =>
                    logSortDirection ==
                        SortDirection.Ascending
                            ? logs.OrderBy(
                                l => l.AverageTotalRms)
                            : logs.OrderByDescending(
                                l => l.AverageTotalRms),

                "BestRms" =>
                    logSortDirection ==
                        SortDirection.Ascending
                            ? logs.OrderBy(
                                l => l.BestTotalRms)
                            : logs.OrderByDescending(
                                l => l.BestTotalRms),

                "WorstRms" =>
                    logSortDirection ==
                        SortDirection.Ascending
                            ? logs.OrderBy(
                                l => l.WorstTotalRms)
                            : logs.OrderByDescending(
                                l => l.WorstTotalRms),

                "Diagnoses" =>
                    logSortDirection ==
                        SortDirection.Ascending
                            ? logs.OrderBy(
                                l => l.DiagnosisCount)
                            : logs.OrderByDescending(
                                l => l.DiagnosisCount),

                "Quality" =>
                    logSortDirection ==
                    SortDirection.Ascending
                    ? logs.OrderBy(l => l.Quality)
                    : logs.OrderByDescending(l => l.Quality),

                _ =>
                  logs
            };


        SelectedFolder.Logs.Clear();

        foreach (var log in sorted)
            SelectedFolder.Logs.Add(log);

        OnPropertyChanged(nameof(DateHeading));
        OnPropertyChanged(nameof(SessionsHeading));
        OnPropertyChanged(nameof(AverageRmsHeading));
        OnPropertyChanged(nameof(BestRmsHeading));
        OnPropertyChanged(nameof(WorstRmsHeading));
        OnPropertyChanged(nameof(DiagnosesHeading));
        OnPropertyChanged(nameof(QualityHeading));
    }
}