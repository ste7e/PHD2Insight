namespace PHD2Insight.UI.ViewModels;

public class MainWindowViewModel : ViewModelBase {

    public ExplorerViewModel Explorer {
        get;
    }


    public MainWindowViewModel() {

        Explorer =
            new ExplorerViewModel();
    }
}