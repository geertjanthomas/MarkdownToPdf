// This file is a part of MarkdownToPdf Library by Geert-Jan Thomas based on earlier work by Tomas Kubec
// Distributed under MIT license - see license.txt
//

namespace VectorAi.MarkdownToPdf.Converters.InlineConverters;

/// <summary>
/// Arguments of <see cref="MarkdownToPdf.ConvertingLiteral"/> Event invoked just before conversion of a literal text span starts.
/// The text can be modified by the event handler and used by the converter afterwars.
/// </summary>
public class ConvertingLiteralEventArgs : EventArgs
{
    public required string Text { get; set; }
}
