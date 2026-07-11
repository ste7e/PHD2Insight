using Avalonia.Controls;
using PHD2Insight.UI.ViewModels;


namespace PHD2Insight.UI;


public partial class MainWindow : Window {

    public MainWindow() {
        InitializeComponent();

        DataContext =
            new MainWindowViewModel();
    }

}