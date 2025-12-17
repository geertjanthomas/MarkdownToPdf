// This file is a part of MarkdownToPdf Library by Geert-Jan Thomas based on earlier work by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using Markdig.Syntax;
using VectorAi.MarkdownToPdf.Converters.ContainerConverters;
using VectorAi.MarkdownToPdf.Styling;
using MigraDoc.DocumentObjectModel;

namespace VectorAi.MarkdownToPdf.Converters.LeafConverters;

internal class HeadingBlockConverter : LeafBlockConverter<HeadingBlock>
{
    internal HeadingBlockConverter(HeadingBlock block, ContainerBlockConverter parent)
        : base(block, parent)
    {
        ElementDescriptor = new SingleElementDescriptor
        {
            Attributes = Attributes,
            Type = ElementType.Heading1 + CurrentBlock.Level - 1,
            Position = new ElementPosition(Block),
            PlainText = GetPlainText(CurrentBlock)
        };
        Attributes.Markup = block.IsSetext ? "Setext" : "Atx";
    }

    protected override void ApplyStyling()
    {
        base.ApplyStyling();
        if (OutputParagraph.Format != null)
        {
            if (Attributes["outline"] == "false") OutputParagraph.Format.OutlineLevel = 0;
            if (Attributes["outline"] == "true") OutputParagraph.Format.OutlineLevel = (OutlineLevel)CurrentBlock.Level;
        }
    }

    protected override void ConvertContent()
    {
        if (Attributes.Id != null)
        {
            OutputParagraph.AddBookmark(Attributes.Id.TrimStart('#'));
        }
        else
        {
            // automatic bookmark - hamburger format
            var plain = GetPlainText(CurrentBlock).Replace(' ', '-');
            OutputParagraph.AddBookmark(plain);
        }

        base.ConvertContent();
    }
}
