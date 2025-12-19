// This file is a part of MarkdownToPdf Library by Geert-Jan Thomas based on earlier work by Tomas Kubec
// Distributed under MIT license - see license.txt
//

namespace VectorAi.MarkdownToPdf.Styling;

/// <summary>
/// Descriptor for single markdown element containing type, attributes and it's position in markdown document tree
/// </summary>
public class SingleElementDescriptor
{
    public ElementAttributes Attributes { get; internal set; } = new ElementAttributes("");
    public ElementType Type { get; internal set; }

    public ElementPosition? Position { get; internal set; }

    /// <summary>
    /// Text of leaf block and its children converted to plain text. For other elements null.
    /// </summary>
    public string? PlainText { get; internal set; }
}
