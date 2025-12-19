// This file is a part of MarkdownToPdf Library by Geert-Jan Thomas based on earlier work by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using Markdig.Syntax;
using VectorAi.MarkdownToPdf.Converters.ContainerConverters;
using VectorAi.MarkdownToPdf.Styling;
using VectorAi.MarkdownToPdf.Utils;
using MigraDoc.DocumentObjectModel;

namespace VectorAi.MarkdownToPdf.Converters.LeafConverters;
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
        var elements = OutputContainer?.Section?.Elements;
        if (elements == null || elements.Count < 2) return;
        var prev = elements[elements.Count - 2];

        if (prev is Paragraph par) par.Format.KeepWithNext = true;
    }

    protected override void ConvertContent()
    {
        OutputParagraph.Format = EvaluatedStyle.Merge(OutputParagraph.Format!, FontSize, Width);
        if (!string.IsNullOrEmpty(EvaluatedStyle.Bullet.Normal.Content))
        {
            OutputParagraph.Format.Font = EvaluatedStyle.Bullet.Normal.Font.MergeWithFont(OutputParagraph.Format.Font, FontSize, Width, false);
            OutputParagraph.AddText(EvaluatedStyle.Bullet.Normal.Content);
        }
    }
}
