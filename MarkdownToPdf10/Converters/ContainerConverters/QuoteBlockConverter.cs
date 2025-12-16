using Markdig.Syntax;
using MarkdownToPdf10.Styling;

namespace MarkdownToPdf10.Converters.ContainerConverters;

internal class QuoteBlockConverter : ContainerBlockConverter
{
    internal QuoteBlockConverter(QuoteBlock block, ContainerBlockConverter parent)
        : base(block, parent)
    {
        ElementDescriptor = new SingleElementDescriptor { Attributes = Attributes, Type = ElementType.Quote, Position = new ElementPosition(Block) };
    }
}
