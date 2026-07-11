using CommunityToolkit.Mvvm.ComponentModel;
using PHD2Insight.UI.Utilities;


namespace PHD2Insight.UI.ViewModels;


public class MainWindowViewModel : ObservableObject {

    public string Status { get; set; }
        = "No log loaded";


    public string Metrics { get; set; }
        = "";


    public RelayCommand LoadCommand { get; }


    public MainWindowViewModel() {
        LoadCommand = new RelayCommand(Load);
    }


    private void Load() {
        Status = "Log loading will be added in alpha2";

        OnPropertyChanged(nameof(Status));
    }

}