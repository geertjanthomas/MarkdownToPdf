using Markdig.Syntax;
using MarkdownToPdf10.Converters.ContainerConverters;
using MarkdownToPdf10.Styling;
using MarkdownToPdf10.Utils;
using MigraDoc.DocumentObjectModel;

namespace MarkdownToPdf10.Converters.LeafConverters;
internal class ThematicBreakBlockConverter : LeafBlockConverter<ThematicBreakBlock>
{
    internal ThematicBreakBlockConverter(ThematicBreakBlock block, ContainerBlockConverter parent)
        : base(block, parent)
    {
        ElementDescriptor = new SingleElementDescriptor { Attributes = Attributes, Type = ElementType.Break, Position = new ElementPosition(Block) };
        Attributes.Markup = block.ThematicChar.ToString();
    }

    protected override void ApplyStyling()
    {
        base.ApplyStyling();

        // prevent thematic break from being on top of a page
        var elements = OutputContainer.Section?.Elements;
        if (elements == null || elements.Count < 2) return;
        var prev = elements[elements.Count - 2];
        if (!(prev is Paragraph)) return;

        (prev as Paragraph).Format.KeepWithNext = true;
    }

    protected override void ConvertContent()
    {
        OutputParagraph.Format = EvaluatedStyle.Merge(OutputParagraph.Format, FontSize, Width);
        if (EvaluatedStyle.Bullet.Normal.Content.HasValue())
        {
            OutputParagraph.Format.Font = EvaluatedStyle.Bullet.Normal.Font.MergeWithFont(OutputParagraph.Format.Font, FontSize, Width, false);
            OutputParagraph.AddText(EvaluatedStyle.Bullet.Normal.Content);
        }
    }
}
