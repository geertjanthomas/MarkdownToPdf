using Markdig.Extensions.CustomContainers;
using MarkdownToPdf10.Styling;

namespace MarkdownToPdf10.Converters.ContainerConverters;

internal class CustomContainerConverter : ContainerBlockConverter
{
    internal CustomContainerConverter(CustomContainer block, ContainerBlockConverter parent)
        : base(block, parent)
    {
        Attributes.Info = block.Info;
        ElementDescriptor = new SingleElementDescriptor { Attributes = Attributes, Type = ElementType.CustomContainer, Position = new ElementPosition(Block) };
    }
}
