// This file is a part of MarkdownToPdf Library by Geert-Jan Thomas based on earlier work by Tomas Kubec
// Distributed under MIT license - see license.txt
//

namespace VectorAi.MarkdownToPdf;

/// <summary>
/// Arguments of <see cref="MarkdownToPdf.WarningIssued"/> event
/// </summary>
public class WarningEventArgs : EventArgs
{
    /// <summary>
    /// Warning category
    /// </summary>
    public required string Category { get; set; }

    /// <summary>
    ///  Detailed warning message
    /// </summary>
    public required string Message { get; set; }
}
