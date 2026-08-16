namespace PHD2Insight.UI.Services;

public sealed class ExplorerSettings {

    private readonly string settingsFolder;
    private readonly string lastFolderFile;

    public ExplorerSettings() {

        settingsFolder =
            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "PHD2Insight");

        lastFolderFile =
            Path.Combine(
                settingsFolder,
                "last-folder.txt");
    }


    public string? LastFolder {

        get {

            try {

                if (!File.Exists(lastFolderFile))
                    return null;

                var path =
                    File.ReadAllText(lastFolderFile).Trim();

                return string.IsNullOrWhiteSpace(path)
                    ? null
                    : path;

            } catch {

                return null;
            }
        }
    }


    public void SaveLastFolder(string path) {

        try {

            Directory.CreateDirectory(settingsFolder);

            File.WriteAllText(
                lastFolderFile,
                path);

        } catch {

            // Failure to save preferences should never
            // prevent the application from functioning.
        }
    }
}