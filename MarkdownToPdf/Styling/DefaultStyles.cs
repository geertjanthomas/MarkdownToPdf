// This file is a part of MarkdownToPdf Library by Geert-Jan Thomas based on earlier work by Tomas Kubec
// Distributed under MIT license - see license.txt
//

using VectorAi.MarkdownToPdf.Styling.Style;
using MigraDoc.DocumentObjectModel;

namespace VectorAi.MarkdownToPdf.Styling;

internal class DefaultStyles
{
    private readonly StyleManager _styleManager;
    internal double fontSize;
    private double _headingScale;
    internal string fontName;
    private Color _codeBg = Color.FromRgb(240, 240, 240);
    private readonly double _defaultIndent;

    internal DefaultStyles(StyleManager styleManager)
    {
        this._styleManager = styleManager;
        fontName = "Arial";
        fontSize = 11;
        _defaultIndent = 2;
        _headingScale = 1.125;
    }

    internal void CreateBasicStyles()
    {
        _styleManager.AddStyle(MarkdownStyleNames.Undefined);

        // Containers

        _styleManager.AddStyle(MarkdownStyleNames.Root);
        _styleManager.AddStyle(MarkdownStyleNames.UnorderedList);
        _styleManager.AddStyle(MarkdownStyleNames.UnorderedListItem);
        _styleManager.AddStyle(MarkdownStyleNames.OrderedList, MarkdownStyleNames.UnorderedList);
        _styleManager.AddStyle(MarkdownStyleNames.OrderedListItem);
        _styleManager.AddStyle(MarkdownStyleNames.Quote);

        _styleManager.AddStyle(MarkdownStyleNames.FootnoteGroup);
        _styleManager.AddStyle(MarkdownStyleNames.Footnote);
        _styleManager.AddStyle(MarkdownStyleNames.Table);
        _styleManager.AddStyle(MarkdownStyleNames.TableHeader);
        _styleManager.AddStyle(MarkdownStyleNames.TableRowOdd);
        _styleManager.AddStyle(MarkdownStyleNames.TableRowEven);
        _styleManager.AddStyle(MarkdownStyleNames.TableCell);
        _styleManager.AddStyle(MarkdownStyleNames.CustomContainer);

        // Leafblocks

        _styleManager.AddStyle(MarkdownStyleNames.Paragraph);
        _styleManager.AddStyle(MarkdownStyleNames.Code);
        _styleManager.AddStyle(MarkdownStyleNames.Break);

        _styleManager.AddStyle(MarkdownStyleNames.QuoteParagraph, MarkdownStyleNames.Paragraph);
        _styleManager.AddStyle(MarkdownStyleNames.FootnoteParagraph, MarkdownStyleNames.Paragraph);

        _styleManager.AddStyle(MarkdownStyleNames.Image);
        _styleManager.AddStyle(MarkdownStyleNames.Plugin);

        // Inlines

        _styleManager.AddStyle(MarkdownStyleNames.Bold);
        _styleManager.AddStyle(MarkdownStyleNames.Italic);
        _styleManager.AddStyle(MarkdownStyleNames.Hyperlink);
        _styleManager.AddStyle(MarkdownStyleNames.InlineCode);

        _styleManager.AddStyle(MarkdownStyleNames.FootnoteReference);
        _styleManager.AddStyle(MarkdownStyleNames.Subscript);
        _styleManager.AddStyle(MarkdownStyleNames.Superscript);
        _styleManager.AddStyle(MarkdownStyleNames.Cite);
        _styleManager.AddStyle(MarkdownStyleNames.Marked);
        _styleManager.AddStyle(MarkdownStyleNames.Inserted);
        _styleManager.AddStyle(MarkdownStyleNames.Strike);
        _styleManager.AddStyle(MarkdownStyleNames.Index);

        _styleManager.AddStyle(MarkdownStyleNames.InlineImage);
        _styleManager.AddStyle(MarkdownStyleNames.InlinePlugin);

        CreateAndBindHeadings();
    }

    internal void BindBasicStyles()
    {
        // Containers

        _styleManager.ForElement(ElementType.Root).Bind(MarkdownStyleNames.Root);
        _styleManager.ForElement(ElementType.UnorderedList).Bind(MarkdownStyleNames.UnorderedList);
        _styleManager.ForElement(ElementType.OrderedList).Bind(MarkdownStyleNames.UnorderedList);
        _styleManager.ForElement(ElementType.OrderedListItem).Bind(MarkdownStyleNames.OrderedListItem);
        _styleManager.ForElement(ElementType.UnorderedListItem).Bind(MarkdownStyleNames.UnorderedListItem);
        _styleManager.ForElement(ElementType.Quote).Bind(MarkdownStyleNames.Quote);

        _styleManager.ForElement(ElementType.FootnoteGroup).Bind(MarkdownStyleNames.FootnoteGroup);
        _styleManager.ForElement(ElementType.Footnote).Bind(MarkdownStyleNames.Footnote);
        _styleManager.ForElement(ElementType.Table).Bind(MarkdownStyleNames.Table);
        _styleManager.ForElement(ElementType.TableHeader).Bind(MarkdownStyleNames.TableHeader);
        _styleManager.ForElement(ElementType.TableRowEven).Bind(MarkdownStyleNames.TableRowEven);
        _styleManager.ForElement(ElementType.TableRowOdd).Bind(MarkdownStyleNames.TableRowOdd);
        _styleManager.ForElement(ElementType.TableCell).Bind(MarkdownStyleNames.TableCell);

        _styleManager.ForElement(ElementType.CustomContainer).Bind(MarkdownStyleNames.CustomContainer);

        // Leafblocks

        _styleManager.ForElement(ElementType.Paragraph).Bind(MarkdownStyleNames.Paragraph);
        _styleManager.ForElement(ElementType.Break).Bind(MarkdownStyleNames.Break);
        _styleManager.ForElement(ElementType.Code).Bind(MarkdownStyleNames.Code);
        _styleManager.ForElement(ElementType.Image).Bind(MarkdownStyleNames.Image);
        _styleManager.ForElement(ElementType.Plugin).Bind(MarkdownStyleNames.Plugin);
        _styleManager.ForElement(ElementType.Paragraph).WithAncestor(ElementType.Quote).Bind(MarkdownStyleNames.QuoteParagraph);
        _styleManager.ForElement(ElementType.Paragraph).WithParent(ElementType.Footnote).Bind(MarkdownStyleNames.FootnoteParagraph);

        // Inlines

        _styleManager.ForElement(ElementType.Bold).Bind(MarkdownStyleNames.Bold);
        _styleManager.ForElement(ElementType.Italic).Bind(MarkdownStyleNames.Italic);
        _styleManager.ForElement(ElementType.Hyperlink).Bind(MarkdownStyleNames.Hyperlink);
        _styleManager.ForElement(ElementType.InlineCode).Bind(MarkdownStyleNames.InlineCode);

        _styleManager.ForElement(ElementType.FootnoteReference).Bind(MarkdownStyleNames.FootnoteReference);
        _styleManager.ForElement(ElementType.Subscript).Bind(MarkdownStyleNames.Subscript);
        _styleManager.ForElement(ElementType.Superscript).Bind(MarkdownStyleNames.Superscript);
        _styleManager.ForElement(ElementType.Cite).Bind(MarkdownStyleNames.Cite);
        _styleManager.ForElement(ElementType.Marked).Bind(MarkdownStyleNames.Marked);
        _styleManager.ForElement(ElementType.Inserted).Bind(MarkdownStyleNames.Inserted);
        _styleManager.ForElement(ElementType.Strike).Bind(MarkdownStyleNames.Strike);
        _styleManager.ForElement(ElementType.Index).Bind(MarkdownStyleNames.Index);
    }

    internal void InitBasicStyles(bool fullInit = true)
    {
        CascadingStyle style;

        style = _styleManager.Styles[MarkdownStyleNames.Undefined];
        if (fullInit)
        {
            style.Font.Color = Colors.Green;
            style.Font.Underline = Underline.Dotted;
        }

        InitContainers();
        InitLeafBlocks();
        InitInlines();
        InitOrUpdateHeadings(_headingScale, fullInit: true);
    }

    internal void InitOrUpdateHeadings(double scale, bool fullInit)
    {
        _headingScale = scale;
        for (int i = 1; i <= 6; i++)
        {
            var style = _styleManager.Styles["Heading" + i];
            if (fullInit)
            {
                style.Font.Bold = true;
                style.Paragraph.KeepWithNext = true;
                style.Paragraph.OutlineLevel = OutlineLevel.BodyText + i;

                var tocStyle = _styleManager.Styles["Toc" + i];
                tocStyle.Font.Color = Colors.Black;
                tocStyle.Margin.Left = Dimension.FromFontSize(_defaultIndent * (i - 1));
            }

            var fs = Dimension.FromFontSize(Math.Pow(scale, 6 - i));
            style.Font.Size = fs;
            style.Margin.Top = ".8em";
            style.Margin.Bottom = ".5em";
        }
    }

    internal void InitSmartyPants()
    {
        //     This is a left single quote ' -gt; lsquo; x8216
        _styleManager.Owner.ConversionSettings.SmartyPantsMapping[Markdig.Extensions.SmartyPants.SmartyPantType.LeftQuote] = "‘";
        //
        // Summary:
        //     This is a right single quote ' -gt; rsquo; x8217
        _styleManager.Owner.ConversionSettings.SmartyPantsMapping[Markdig.Extensions.SmartyPants.SmartyPantType.RightQuote] = "’";
        //
        //
        // Summary:
        //     This is a left double quote " -gt; ldquo;x8220
        _styleManager.Owner.ConversionSettings.SmartyPantsMapping[Markdig.Extensions.SmartyPants.SmartyPantType.LeftDoubleQuote] = "“";
        //
        // Summary:
        //     This is a right double quote " -gt; rdquo; x8221
        _styleManager.Owner.ConversionSettings.SmartyPantsMapping[Markdig.Extensions.SmartyPants.SmartyPantType.RightDoubleQuote] = "”";
        //
        // Summary:
        //     This is a right double quote << -gt; laquo; x171
        _styleManager.Owner.ConversionSettings.SmartyPantsMapping[Markdig.Extensions.SmartyPants.SmartyPantType.LeftAngleQuote] = "«";
        //
        // Summary:
        //     This is a right angle quote >> -gt; raquo; x187
        _styleManager.Owner.ConversionSettings.SmartyPantsMapping[Markdig.Extensions.SmartyPants.SmartyPantType.RightAngleQuote] = "»";
        //
        // Summary:
        //     This is an ellipsis ... -gt; hellip; \x8230
        _styleManager.Owner.ConversionSettings.SmartyPantsMapping[Markdig.Extensions.SmartyPants.SmartyPantType.Ellipsis] = "…";
        //
        // Summary:
        //     This is a ndash -- -gt; ndash; \x8211
        _styleManager.Owner.ConversionSettings.SmartyPantsMapping[Markdig.Extensions.SmartyPants.SmartyPantType.Dash2] = "–";
        //
        // Summary:
        //     This is a mdash --- -gt; mdash; \x8212
        _styleManager.Owner.ConversionSettings.SmartyPantsMapping[Markdig.Extensions.SmartyPants.SmartyPantType.Dash3] = "—";
    }

    internal void SetDefaultFont(string fontName, double fontSize)
    {
        this.fontName = fontName;
        this.fontSize = fontSize;

        var root = _styleManager.Styles[MarkdownStyleNames.Root];
        root.Font.Name = fontName;
        root.Font.Size = fontSize;
    }

    private void InitContainers()
    {
        CascadingStyle style;

        style = _styleManager.Styles[MarkdownStyleNames.Root];
        style.Font.Name = fontName;
        style.Font.Size = fontSize;

        style = _styleManager.Styles[MarkdownStyleNames.UnorderedList];
        style.Margin.Left = Dimension.FromFontSize(_defaultIndent);
        style.Margin.Top = ".75em";
        style.Margin.Bottom = ".75em";

        style = _styleManager.Styles[MarkdownStyleNames.UnorderedListItem];
        style.Bullet.Normal.Content = "\x2022";
        style.Bullet.Unchecked.Font.Name = "Wingdings";
        style.Bullet.Unchecked.Content = "\xA8";
        style.Bullet.Checked.Font.Name = "Wingdings";
        style.Bullet.Checked.Content = "\xFE";
        style.Bullet.Normal.Font.Bold = false;
        style.Bullet.Normal.Font.Italic = false;
        style.Bullet.Normal.Font.Superscript = false;
        style.Bullet.Normal.Font.Subscript = false;
        style.Bullet.Checked.Font.Bold = false;
        style.Bullet.Checked.Font.Italic = false;
        style.Bullet.Checked.Font.Superscript = false;
        style.Bullet.Checked.Font.Subscript = false;
        style.Bullet.Unchecked.Font.Bold = false;
        style.Bullet.Unchecked.Font.Italic = false;
        style.Bullet.Unchecked.Font.Superscript = false;
        style.Bullet.Unchecked.Font.Subscript = false;
        style.Bullet.TextIndent = Dimension.FromFontSize(_defaultIndent);
        style.Bullet.BulletIndent = Dimension.FromFontSize(_defaultIndent * 0.5);
        style.Margin.Top = ".5em";

        // Currently no other styling for Ordered list then Unordered: style = styleManager.Styles[MarkdownStyleNames.OrderedList]

        style = _styleManager.Styles[MarkdownStyleNames.OrderedListItem];
        style.Bullet.TextIndent = Dimension.FromFontSize(_defaultIndent);
        style.Margin.Top = ".5em";
        style.Bullet.Normal.Content = ".";
        style.Bullet.BulletIndent = Dimension.FromFontSize(_defaultIndent * 0.75);

        style = _styleManager.Styles[MarkdownStyleNames.Quote];
        style.Font.Color = Color.FromRgb(80, 80, 80);
        style.Background = Colors.White;
        style.Margin.Top = "1em";
        style.Margin.Bottom = "1em";
        style.Padding.Top = "1em";
        style.Padding.Bottom = ".5em";
        style.Margin.Left = Dimension.FromFontSize(_defaultIndent);

        style = _styleManager.Styles[MarkdownStyleNames.Footnote];
        style.Bullet.Normal.Content = ".";
        style.Bullet.TextIndent = Dimension.FromFontSize(_defaultIndent);
        style.Bullet.BulletIndent = Dimension.FromFontSize(_defaultIndent * 0.8);

        style = _styleManager.Styles[MarkdownStyleNames.FootnoteGroup];
        style.Margin.Top = Unit.FromPoint(fontSize);

        style = _styleManager.Styles[MarkdownStyleNames.Table];
        style.Margin.Top = "1em";
        style.Margin.Bottom = "1em";
        style.Table.CellSpacing = ".5em";
        style.Table.CellSpacing.Bottom = "0em";
        style.Border.Width = .8;
        style.Border.Color = Colors.Gray;

        style = _styleManager.Styles[MarkdownStyleNames.TableHeader];
        style.Font.Bold = true;
        style.Border.Bottom.Width = .8;
        style.Border.Bottom.Color = Colors.Gray;

        style = _styleManager.Styles[MarkdownStyleNames.TableCell];
        style.Border.Width = .4;
        style.Border.Color = Colors.Gray;

        style.Border.Bottom.Width = new Dimension();
        style.Border.Bottom.Color = Color.Empty;

        style = _styleManager.Styles[MarkdownStyleNames.CustomContainer];
        style.Background = Colors.AliceBlue;
        style.Padding.Left = "1em";
        style.Padding.Right = "1em";
        style.Padding.Top = "1em";
        style.Padding.Bottom = "1em";
        style.Margin.Top = "1em";
        style.Margin.Bottom = "1em";
    }

    private void InitInlines()
    {
        CascadingStyle style;
        style = _styleManager.Styles[MarkdownStyleNames.Bold];
        style.Font.Bold = true;

        style = _styleManager.Styles[MarkdownStyleNames.Italic];
        style.Font.Italic = true;

        style = _styleManager.Styles[MarkdownStyleNames.Hyperlink];
        style.Font.Color = Colors.Blue;

        style = _styleManager.Styles[MarkdownStyleNames.InlineCode];
        style.Font.Name = "Consolas";
        style.Font.Color = Colors.Chocolate;

        style = _styleManager.Styles[MarkdownStyleNames.FootnoteReference];
        style.Font.Superscript = true;

        style = _styleManager.Styles[MarkdownStyleNames.Superscript];
        style.Font.Superscript = true;

        style = _styleManager.Styles[MarkdownStyleNames.Subscript];
        style.Font.Subscript = true;

        style = _styleManager.Styles[MarkdownStyleNames.Cite];
        style.Font.Underline = Underline.Dotted;

        style = _styleManager.Styles[MarkdownStyleNames.Marked];
        style.Font.Color = Colors.Red;
        style.Font.Bold = true;

        style = _styleManager.Styles[MarkdownStyleNames.Inserted];
        style.Font.Color = Colors.Green;

        style = _styleManager.Styles[MarkdownStyleNames.Strike];
        style.Font.Color = Colors.Gray;
        style.Font.Italic = true;

        // Special inlines

        style = _styleManager.Styles[MarkdownStyleNames.Index];
        style.Font.Color = Colors.Black;
    }

    private void InitLeafBlocks()
    {
        CascadingStyle style;
        style = _styleManager.Styles[MarkdownStyleNames.Paragraph];
        style.Paragraph.WidowControl = true;
        style.Margin.Bottom = ".75em";

        style = _styleManager.Styles[MarkdownStyleNames.Break];
        style.Border.Bottom.Color = Colors.Gray;
        style.Border.Bottom.Width = .25;
        style.Margin.Bottom = "1.5em";

        style = _styleManager.Styles[MarkdownStyleNames.QuoteParagraph];
        style.Border.Left.LineStyle = MigraDoc.DocumentObjectModel.BorderStyle.Single;
        style.Border.Left.Color = Colors.LightGray;
        style.Padding.Left = Dimension.FromFontSize(_defaultIndent * 0.5);
        style.Padding.Bottom = style.Margin.Bottom;
        style.Border.Left.Width = ".25em";
        style.Margin.Bottom = 0;

        style = _styleManager.Styles[MarkdownStyleNames.Code];
        style.Font.Name = "Consolas";
        style.Font.Size = "1em";
        style.Background = _codeBg;
        style.Margin.Bottom = fontSize;
        style.Margin.Top = ".5em";
        style.Padding.Bottom = ".5em";
        style.Padding.Left = ".5em";
        style.Padding.Top = ".5em";
        style.Padding.Right = ".5em";

        style = _styleManager.Styles[MarkdownStyleNames.Image];

        style.Paragraph.Alignment = ParagraphAlignment.Center;
        style.Margin.Top = "1em";
        style.Margin.Bottom = "1em";

        style = _styleManager.Styles[MarkdownStyleNames.Plugin];

        style.Paragraph.Alignment = ParagraphAlignment.Center;
        style.Margin.Top = "1em";
        style.Margin.Bottom = "1em";
    }

    private void CreateAndBindHeadings()
    {
        for (int i = 1; i <= 6; i++)
        {
            var style = _styleManager.AddStyle("Heading" + i);
            _styleManager.ForElement(ElementType.Heading1 + i - 1).Bind(style);

            style = _styleManager.AddStyle("Toc" + i);
            _styleManager.ForElement(ElementType.Toc1 + i - 1).Bind(style);
        }
    }
}
