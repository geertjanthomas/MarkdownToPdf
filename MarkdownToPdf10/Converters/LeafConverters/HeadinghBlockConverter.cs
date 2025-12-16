using Markdig.Syntax;
using MarkdownToPdf10.Converters.ContainerConverters;
using MarkdownToPdf10.Styling;
using MarkdownToPdf10.Utils;
using MigraDoc.DocumentObjectModel;

namespace MarkdownToPdf10.Converters.LeafConverters;

internal class HeadinghBlockConverter : LeafBlockConverter<HeadingBlock>
{
    internal HeadinghBlockConverter(HeadingBlock block, ContainerBlockConverter parent)
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
        if (Attributes["outline"] == "false") OutputParagraph.Format.OutlineLevel = 0;
        if (Attributes["outline"] == "true") OutputParagraph.Format.OutlineLevel = (OutlineLevel)CurrentBlock.Level;
    }

    protected override void ConvertContent()
    {
        if (Attributes.Id.HasValue())
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
