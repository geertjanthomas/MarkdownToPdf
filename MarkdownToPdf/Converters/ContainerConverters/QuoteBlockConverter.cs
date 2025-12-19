// This file is a part of MarkdownToPdf Library by Geert-Jan Thomas based on earlier work by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using Markdig.Syntax;
using VectorAi.MarkdownToPdf.Styling;

namespace VectorAi.MarkdownToPdf.Converters.ContainerConverters;

internal class QuoteBlockConverter : ContainerBlockConverter
{
    internal QuoteBlockConverter(QuoteBlock block, ContainerBlockConverter parent)
        : base(block, parent)
    {
        ElementDescriptor = new SingleElementDescriptor { Attributes = Attributes, Type = ElementType.Quote, Position = new ElementPosition(Block) };
    }
}
