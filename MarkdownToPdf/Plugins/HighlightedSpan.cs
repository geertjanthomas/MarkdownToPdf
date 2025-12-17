// This file is a part of MarkdownToPdf Library by Geert-Jan Thomas based on earlier work by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using System.Drawing;

namespace VectorAi.MarkdownToPdf.Plugins;

/// <summary>
/// Text span with formatting
/// </summary>
public class HighlightedSpan
{
    public required string Text { get; set; }
    public bool Bold { get; set; }
    public bool Italic { get; set; }
    public bool Underline { get; set; }
    public Color Color { get; set; }
}
