namespace MarkdownToPdf10;

/// <summary>
/// Arguments of <see cref="MarkdownToPdf.WarningIssued"/> event
/// </summary>
public class WarningEventArgs : EventArgs
{
    /// <summary>
    /// Warning category
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    ///  Detailed warning message
    /// </summary>
    public string Message { get; set; }
}
