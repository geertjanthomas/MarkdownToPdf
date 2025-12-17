// This file is a part of MarkdownToPdf Library by Geert-Jan Thomas based on earlier work by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using Markdig.Syntax;
using VectorAi.MarkdownToPdf.Styling;

namespace VectorAi.MarkdownToPdf.Converters.ContainerConverters;

internal class ListItemConverter : ContainerBlockConverter
{
    internal ListItemConverter(ListItemBlock block, ContainerBlockConverter parent)
        : base(block, parent)
    {
        ElementDescriptor = new SingleElementDescriptor
        {
            Attributes = Attributes,
            Type = Parent?.ElementDescriptor.Type == ElementType.OrderedList ? ElementType.OrderedListItem : ElementType.UnorderedListItem,
            Position = new ElementPosition(Block)
        };
    }
}
