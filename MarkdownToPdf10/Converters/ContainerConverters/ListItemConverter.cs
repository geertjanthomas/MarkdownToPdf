using Markdig.Syntax;
using MarkdownToPdf10.Styling;

namespace MarkdownToPdf10.Converters.ContainerConverters;

internal class ListItemConverter : ContainerBlockConverter
{
    internal ListItemConverter(ListItemBlock block, ContainerBlockConverter parent)
        : base(block, parent)
    {
        ElementDescriptor = new SingleElementDescriptor
        {
            Attributes = Attributes,
            Type = Parent.ElementDescriptor.Type == ElementType.OrderedList ? ElementType.OrderedListItem : ElementType.UnorderedListItem,
            Position = new ElementPosition(Block)
        };
    }
}
