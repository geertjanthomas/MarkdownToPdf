// This file is a part of MarkdownToPdf Library by Geert-Jan Thomas based on earlier work by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using Markdig.Extensions.Footnotes;
using Markdig.Syntax;
using VectorAi.MarkdownToPdf.Styling;

namespace VectorAi.MarkdownToPdf.Converters.ContainerConverters;

internal class FootnoteGroupConverter : ContainerBlockConverter
{
    internal FootnoteGroupConverter(FootnoteGroup block, ContainerBlockConverter parent)
        : base(block, parent)
    {
        ElementDescriptor = new SingleElementDescriptor { Attributes = Attributes, Type = ElementType.FootnoteGroup, Position = new ElementPosition(Block) };
    }

    protected override bool ConvertBlock(Block block)
    {
        if (block is Footnote footnote)
        {
            var conv = new FootnoteConverter(footnote, this);
            conv.Convert();
            return true;
        }

        return false;
    }
}
