using Markdig.Syntax.Inlines;
using MarkdownToPdf10.MigrDoc;

namespace MarkdownToPdf10.Converters.InlineConverters;

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
    IInlineConverter ParentInlineConverter { get; }
}
