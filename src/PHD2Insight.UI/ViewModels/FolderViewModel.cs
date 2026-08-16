using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PHD2Insight.UI.ViewModels;

public partial class FolderViewModel :
    ViewModelBase {

    public FolderViewModel(
        string name,
        string path) {

        Name = name;
        Path = path;
    }


    public string Name {
        get;
    }


    public string Path {
        get;
    }


    public ObservableCollection<LogViewModel> Logs {
        get;
    } = [];


    [ObservableProperty]
    private bool isExpanded = true;
}