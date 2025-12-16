using System.Drawing;

namespace MarkdownToPdf10.Plugins;

/// <summary>
/// Text span with formatting
/// </summary>
public class HighlightedSpan
{
    public string Text { get; set; }
    public bool Bold { get; set; }
    public bool Italic { get; set; }
    public bool Underline { get; set; }
    public Color Color { get; set; }
}
