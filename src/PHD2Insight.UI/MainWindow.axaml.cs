using Avalonia.Controls;
using Avalonia.Platform.Storage;
using PHD2Insight.UI.ViewModels;

namespace PHD2Insight.UI;

public partial class MainWindow : Window {

    private readonly MainWindowViewModel viewModel;

    public MainWindow() {

        InitializeComponent();

        viewModel =
            new MainWindowViewModel();

        DataContext =
            viewModel;

        viewModel.Explorer.RequestFolderAsync =
            SelectFolderAsync;
    }


    private async Task<string?> SelectFolderAsync() {

        var folders =
            await StorageProvider.OpenFolderPickerAsync(
                new FolderPickerOpenOptions {
                    Title = "Select PHD2 Log Folder",
                    AllowMultiple = false
                });

        return folders
            .FirstOrDefault()
            ?.Path
            .LocalPath;
    }
}