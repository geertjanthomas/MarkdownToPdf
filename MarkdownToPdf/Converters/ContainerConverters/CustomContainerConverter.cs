// This file is a part of MarkdownToPdf Library by Geert-Jan Thomas based on earlier work by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using Markdig.Extensions.CustomContainers;
using VectorAi.MarkdownToPdf.Styling;

namespace VectorAi.MarkdownToPdf.Converters.ContainerConverters;

internal class CustomContainerConverter : ContainerBlockConverter
{
    internal CustomContainerConverter(CustomContainer block, ContainerBlockConverter parent)
        : base(block, parent)
    {
        Attributes.Info = block.Info;
        ElementDescriptor = new SingleElementDescriptor { Attributes = Attributes, Type = ElementType.CustomContainer, Position = new ElementPosition(Block) };
    }
}
