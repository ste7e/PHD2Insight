using System.Diagnostics;

namespace PHD2Insight.Parser.Infrastructure;

/// <summary>
/// Reads text from a stream one line at a time while tracking the current line number.
/// </summary>
public sealed class LineReader : IDisposable {
    private readonly StreamReader _reader;

    public LineReader(Stream stream) {
        ArgumentNullException.ThrowIfNull(stream);

        _reader = new StreamReader(
            stream,
            leaveOpen: true);
    }

    /// <summary>
    /// Gets the current line number.
    /// The first line in the file is line 1.
    /// </summary>
    public int LineNumber { get; private set; }

    /// <summary>
    /// Gets the current line that was read.
    /// Returns null before the first line and after end-of-file.
    /// </summary>
    public string? CurrentLine { get; private set; }

    /// <summary>
    /// Reads the next line.
    /// </summary>
    /// <returns>
    /// True if another line was read; otherwise false.
    /// </returns>
    public bool Read() {
        CurrentLine = _reader.ReadLine();

        if (CurrentLine is null)
            return false;

        LineNumber++;

        return true;
    }

    public void Dispose() {
        _reader.Dispose();
    }
}