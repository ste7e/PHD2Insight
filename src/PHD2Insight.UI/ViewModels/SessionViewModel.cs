using CommunityToolkit.Mvvm.ComponentModel;
using PHD2Insight.Analysis.Diagnostics;
using PHD2Insight.Analysis.Models;
using PHD2Insight.Core.Models;
using PHD2Insight.UI.ViewModels;

public sealed partial class SessionViewModel :
    ViewModelBase {

    public SessionViewModel(
        int sessionNumber,
        GuidingSession session,
        AnalysisResult analysis,
        IReadOnlyList<Diagnosis> diagnoses,
        GuidingQuality quality) {

        SessionNumber = sessionNumber;
        Session = session;
        Analysis = analysis;
        Diagnoses = diagnoses;
        Quality = quality;
    }

    public int SessionNumber {
        get;
    }

    public GuidingQuality Quality {
        get;
    }

    public string QualityDisplay =>
    Quality switch {
        GuidingQuality.Good => "Good",
        GuidingQuality.Acceptable => "Acceptable",
        GuidingQuality.Poor => "Poor",
        _ => "Unknown"
    };

    public GuidingSession Session {
        get;
    }


    public AnalysisResult Analysis {
        get;
    }


    public IReadOnlyList<Diagnosis> Diagnoses {
        get;
    }


    public double? TotalRmsValue =>
        Analysis.Rms?.TotalArcSeconds;


    public string TotalRms =>
        TotalRmsValue.HasValue
            ? $"{TotalRmsValue.Value:F2}\""
            : "—";


    // date is missing from this as it can be inferred from the log header
    public string StartTime =>
        Session.StartTime.ToString(
            "HH:mm:ss");


    public string Duration {

        get {

            if (Session.EndTime is null)
                return "—";

            var duration =
                Session.EndTime.Value -
                Session.StartTime;

            return duration.TotalMinutes >= 60
                ? duration.ToString(@"h\:mm\:ss")
                : duration.ToString(@"m\:ss");
        }
    }


    public string Camera =>
        Session.Camera?.Name ?? "—";


    public string Mount =>
        Session.Mount?.Name ?? "—";


    public string Exposure =>
        $"{Session.ExposureMilliseconds} ms";


    public string PixelScale =>
        $"{Session.PixelScale:F2}\"/px";


    public string FocalLength =>
        $"{Session.FocalLengthMm} mm";


    public string Binning =>
        Session.Binning.ToString();

    public string SessionTitle =>
    $"{SessionNumber}";

    public string DurationDisplay =>
        $"{Duration}";

    public string TotalRmsDisplay =>
        $"{TotalRms}";

    public string RaRmsDisplay =>
        Analysis.Rms?.RaArcSeconds is double value
            ? $"{value:F2}\""
            : "—";

    public string DecRmsDisplay =>
        Analysis.Rms?.DecArcSeconds is double value
            ? $"{value:F2}\""
            : "—";

    public string DiagnosisCountDisplay =>
        Diagnoses.Count == 1
            ? "1"
            : $"{Diagnoses.Count}";

    [ObservableProperty]
    private bool isDetailOpen;
}