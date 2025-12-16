using Markdig.Syntax;
using MarkdownToPdf10.Styling;
using MarkdownToPdf10.Utils;

namespace MarkdownToPdf10.Converters.ContainerConverters;

internal class ListConverter : ContainerBlockConverter
{
    internal ListConverter(ListBlock block, ContainerBlockConverter parent)
        : base(block, parent)
    {
        ElementDescriptor = new SingleElementDescriptor
        {
            Attributes = Attributes,
            Type = block.OrderedStart == null ? ElementType.UnorderedList : ElementType.OrderedList
        };
        Attributes.Markup = block.OrderedStart.HasValue() ? "Number" : block.BulletType.ToString();
    }
}
