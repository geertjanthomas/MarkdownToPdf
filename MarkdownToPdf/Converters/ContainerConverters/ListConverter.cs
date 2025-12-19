// This file is a part of MarkdownToPdf Library by Geert-Jan Thomas based on earlier work by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using Markdig.Syntax;
using VectorAi.MarkdownToPdf.Styling;

namespace VectorAi.MarkdownToPdf.Converters.ContainerConverters;

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
        Attributes.Markup = block.OrderedStart != null ? "Number" : block.BulletType.ToString();
    }
}
