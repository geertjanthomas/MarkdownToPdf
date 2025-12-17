// This file is a part of MarkdownToPdf Library by Geert-Jan Thomas based on earlier work by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using Markdig.Extensions.TaskLists;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using VectorAi.MarkdownToPdf.Converters.ContainerConverters;
using VectorAi.MarkdownToPdf.Converters.InlineConverters;
using VectorAi.MarkdownToPdf.Styling;
using VectorAi.MarkdownToPdf.Utils;
using MigraDoc.DocumentObjectModel;

namespace VectorAi.MarkdownToPdf.Converters.LeafConverters;

internal class ParagraphBlockConverter : LeafBlockConverter<ParagraphBlock>
{
    internal ParagraphBlockConverter(ParagraphBlock block, ContainerBlockConverter parent)
        : base(block, parent)
    {
        ElementDescriptor = new SingleElementDescriptor
        {
            Attributes = Attributes,
            Type = ElementType.Paragraph,
            Position = new ElementPosition(Block),
            PlainText = GetPlainText(CurrentBlock)
        };
    }

    protected override void PrepareStyling()
    {
        if (CurrentBlock.Inline?.FirstOrDefault() is LinkInline link && link.IsImage)
        {
            ElementDescriptor.Type = ElementType.Image;
            if (link.Url?.StartsWith("md:plugin") ?? false)
            {
                ElementDescriptor.Type = ElementType.Plugin;
            }
        }

        if (CurrentBlock.Inline?.FirstOrDefault() is Markdig.Extensions.Mathematics.MathInline)
        {
            ElementDescriptor.Type = ElementType.Plugin;
        }

        base.PrepareStyling();
    }

    protected override bool CreateOutput()
    {
        if (TryTreatAsCommandParagraph()) return false;

        base.CreateOutput();
        return true;
    }

    protected override void ConvertContent()
    {
        AddFootnoteBookmark();
        AddBullet();
        base.ConvertContent();
    }

    private void AddBullet()
    {
        if (Parent?.ElementDescriptor.Type != ElementType.UnorderedListItem && Parent?.ElementDescriptor.Type != ElementType.OrderedListItem
            && Parent?.ElementDescriptor.Type != ElementType.Footnote) return;

        var bultStyle = Parent.EvaluatedStyle.Bullet;
        OutputParagraph.Format?.FirstLineIndent = -bultStyle.TextIndent.Eval(FontSize, Width);
        var indent = OutputParagraph.Format?.LeftIndent ?? 0;
        OutputParagraph.Format?.LeftIndent += bultStyle.TextIndent.Eval(FontSize, Width);
        OutputParagraph.Format?.ClearAll();
        OutputParagraph.Format?.AddTabStop(indent + bultStyle.BulletIndent.Eval(FontSize, Width), TabAlignment.Right);
        OutputParagraph.Format?.AddTabStop(indent + bultStyle.TextIndent.Eval(FontSize, Width), TabAlignment.Left);

        if (Block.GetIndex() == 0)
        {
            string bullet;
            Font bulFont;
            if (Parent.Parent?.ElementDescriptor.Type == ElementType.OrderedList)
            {
                if (!int.TryParse((Parent.Parent.Block as ListBlock)!.OrderedStart, out int start)) start = 1;
                bullet = Parent.Block.GetIndex() + start + bultStyle.Normal.Content;
                bulFont = bultStyle.Normal.Font.MergeWithFont(OutputParagraph.Format!.Font, Parent.FontSize, Parent.Width, alreadyScaled: true);
            }
            else if (Parent.ElementDescriptor.Type == ElementType.Footnote)
            {
                bullet = Parent.Block.GetIndex() + 1 + bultStyle.Normal.Content;
                bulFont = bultStyle.Normal.Font.MergeWithFont(OutputParagraph.Format!.Font, Parent.FontSize, Parent.Width, alreadyScaled: true);
            }
            else
            {
                if (CurrentBlock.Inline?.Any(x => x.GetType() == typeof(TaskList)) ?? false)
                {
                    var taskList = CurrentBlock.Inline.First(x => x.GetType() == typeof(TaskList)) as TaskList;
                    var taskListChecked = taskList?.Checked ?? false;
                    bullet = taskListChecked ? bultStyle.Checked.Content : bultStyle.Unchecked.Content;
                    bulFont = taskListChecked
                        ? bultStyle.Checked.Font.MergeWithFont(OutputParagraph.Format!.Font, Parent.FontSize, Parent.Width, alreadyScaled: true)
                        : bultStyle.Unchecked.Font.MergeWithFont(OutputParagraph.Format!.Font, Parent.FontSize, Parent.Width, alreadyScaled: true);
                }
                else
                {
                    bullet = bultStyle.Normal.Content;
                    bulFont = bultStyle.Normal.Font.MergeWithFont(OutputParagraph.Format!.Font, Parent.FontSize, Parent.Width, alreadyScaled: true);
                }
            }

            OutputParagraph.AddFormattedText($"\t{bullet}\t", bulFont);
        }
        else
        {
            OutputParagraph.AddText("\t\t");
        }
    }

    private void AddFootnoteBookmark()
    {
        if (Parent?.ElementDescriptor.Type != ElementType.Footnote) return;
        if (Block.GetIndex() != 0) return;
        var num = Parent.Block.GetIndex() + 1;
        OutputParagraph.AddBookmark("Footnote_" + num);
    }

    #region Special command field paragraphs

    private bool TryTreatAsCommandParagraph()
    {
        if (IsCommandParagraph(CurrentBlock) && CurrentBlock.Inline?.FirstChild != null)
        {
            return ConvertCommandParagraph(CurrentBlock.Inline.FirstChild);
        }
        return false;
    }

    private static bool IsCommandParagraph(ParagraphBlock paragraphBlock)
    {
        return ((paragraphBlock.Inline?.Count() ?? 0) == 1) && paragraphBlock.Inline?.FirstChild != null && InlineConverter.IsSpecialField(paragraphBlock.Inline.FirstChild);
    }

    private bool ConvertCommandParagraph(Inline inline)
    {
        var cmd = InlineConverter.ParseSpecialField(inline);
        if (cmd == null || !cmd.Any()) return false;

        switch (cmd[0].Key.ToLower())
        {
            case "pagebreak":
                {
                    OutputContainer?.AddPageBreak();
                    return true;
                }
            case "sectionbreak":
                {
                    OutputContainer?.AddSectionBreak();
                    return true;
                }

            default:
                {
                    return false;
                }
        }
    }
}

#endregion Special command field paragraphs
