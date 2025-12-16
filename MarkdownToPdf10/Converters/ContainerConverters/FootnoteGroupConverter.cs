using Markdig.Extensions.Footnotes;
using Markdig.Syntax;
using MarkdownToPdf10.Styling;

namespace MarkdownToPdf10.Converters.ContainerConverters;

internal class FootnoteGroupConverter : ContainerBlockConverter
{
    internal FootnoteGroupConverter(FootnoteGroup block, ContainerBlockConverter parent)
        : base(block, parent)
    {
        ElementDescriptor = new SingleElementDescriptor { Attributes = Attributes, Type = ElementType.FootnoteGroup, Position = new ElementPosition(Block) };
    }

    protected override bool ConvertBlock(Block block)
    {
        var type = block.GetType();

        if (type == typeof(Markdig.Extensions.Footnotes.Footnote))
        {
            var conv = new FootnoteConverter(block as Markdig.Extensions.Footnotes.Footnote, this);
            conv.Convert();
            return true;
        }

        return false;
    }
}
