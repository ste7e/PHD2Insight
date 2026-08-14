using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using PHD2Insight.UI.ViewModels;


namespace PHD2Insight.UI;


public partial class MainWindow :
    Window {

    private readonly MainWindowViewModel viewModel;


    public MainWindow() {
        InitializeComponent();

        viewModel =
            new MainWindowViewModel();

        DataContext =
            viewModel;
    }


    private async void LoadLog(
        object? sender,
        RoutedEventArgs e) {

        var options = new FilePickerOpenOptions {
            Title = "Open PHD2 Guide Log",
            AllowMultiple = false,
            FileTypeFilter = [
                new FilePickerFileType("PHD2 Guide Logs") {
                    Patterns = ["*.txt"]
                },
                new FilePickerFileType("All Files") {
                    Patterns = ["*"]
                }
            ]
        };

        var files =
            await StorageProvider.OpenFilePickerAsync(options);

        if (files.Count > 0) {
            var filePath =
                files[0].TryGetLocalPath();

            if (!string.IsNullOrWhiteSpace(filePath)) {
                viewModel.LoadLog(filePath);
            }
        }
    }
}