using System;
using System.Collections.Generic;

namespace MarkdownToPdf10.Converters.InlineConverters;

/// <summary>
/// Arguments of <see cref="MarkdownToPdf.ConvertingLiteral"/> Event invoked just before conversion of a literal text span starts.
/// The text can be modified by the event handler and used by the converter afterwars.
/// </summary>
public class ConvertingLiteralEventArgs : EventArgs
{
    public string Text { get; set; }
}
