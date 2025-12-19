// This file is a part of MarkdownToPdf Library by Geert-Jan Thomas based on earlier work by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using Markdig.Syntax.Inlines;
using VectorAi.MarkdownToPdf.MigrDoc;

namespace VectorAi.MarkdownToPdf.Converters.InlineConverters;

/// <summary>
///  Common interface for all types of InlineConverters
/// </summary>

public interface IInlineConverter : IElementConverter
{
    /// <summary>
    /// Current Markdig inline element
    /// </summary>
    Inline Inline { get; }

    /// <summary>
    /// MigraDoc paragraph (or hyperlink content) that is currently being created
    /// </summary>
    MigrDocInlineContainer OutputParagraph { get; }

    /// <summary>
    /// InlineConverter that owns this inline - applies to nested Inlines. BlockConverter owning this inline and all ancestor inlines is <see cref="IElementConverter.Parent"/>
    /// </summary>
    IInlineConverter? ParentInlineConverter { get; }
}
