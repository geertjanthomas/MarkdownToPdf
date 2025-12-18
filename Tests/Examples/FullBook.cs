using VectorAi.MarkdownToPdf;
using VectorAi.MarkdownToPdf.Styling;
using MigraDoc.DocumentObjectModel;
using System.Runtime.InteropServices;

namespace Test10.Examples;

/// <summary>
/// Demonstration of an entire book
///
/// </summary>

public static class FullBook
{
    public static void Run()
    {
        var defaultFont = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Garamond" : "Times New Roman";
        var filePath = Path.Join(Program.BasePath(),"data/Alice.md");
        var markdown = File.ReadAllText(filePath);
        var footer = "{.center}\r\n[](md:page)";

        var pdf = new MarkdownToPdf();

        DefineStyles(pdf);

        pdf.DefaultPageSetup.SectionStart = BreakType.BreakOddPage;
        pdf
         .PaperSize(PaperSize.B5)
         .DefaultDpi(200)
         .Title("Alice's Adventures in Wonderland")
         .Author("Lewis Carroll")
         .DefaultFont(defaultFont, 12)
         .ImageDir("../../../data/")
         .PageMargins("2cm", "2cm", "2cm", "2.5cm")
         .Add(markdown)
         .AddFooter(footer)
         .Save("book_alice.pdf");
    }

    private static void DefineStyles(MarkdownToPdf pdf)
    {
        var style = pdf.StyleManager.Styles[MarkdownStyleNames.Paragraph];
        style.Paragraph.Alignment = ParagraphAlignment.Justify;
        style.Paragraph.FirstLineIndent = "1cm";
        style.Margin.Bottom = "0.25em";

        style = pdf.StyleManager.Styles[MarkdownStyleNames.Code];
        style.Font = pdf.StyleManager.Styles[MarkdownStyleNames.Paragraph].Font;
        style.Background = Color.Empty;

        style = pdf.StyleManager.Styles[MarkdownStyleNames.Heading1];
        style.Paragraph.PageBreakBefore = true;

        style = pdf.StyleManager.Styles[MarkdownStyleNames.Heading2];
        style.Margin.Top = 0;
        style.Font.Size = pdf.StyleManager.Styles[MarkdownStyleNames.Heading1].Font.Size;
        style.Paragraph.OutlineLevel = 0;

        style = pdf.StyleManager.AddStyle("center", MarkdownStyleNames.Paragraph);
        style.Paragraph.Alignment = ParagraphAlignment.Center;
        style.Paragraph.FirstLineIndent = 0;
        style.Margin.Bottom = "1em";
        pdf.StyleManager.ForElement(ElementType.Paragraph, "center").Bind(style);

        // definition of asterism ruler - centered decorative symbol
        var asterismStyle = pdf.StyleManager.AddStyle("asterismRuler", MarkdownStyleNames.Break);
        asterismStyle.Paragraph.FirstLineIndent = 0;
        asterismStyle.Paragraph.Alignment = ParagraphAlignment.Center;
        asterismStyle.Bullet.Normal.Content = "\xF9";
        asterismStyle.Bullet.Normal.Font.Name = "Wingdings 2";
        asterismStyle.Bullet.Normal.Font.Size = "1.5em";
        asterismStyle.Margin.Top = 12;
        asterismStyle.Margin.Bottom = 12;
        asterismStyle.Border.Bottom.Width = 0;
        pdf.StyleManager
            .ForElement(ElementType.Break)
            .Where(x => x.CurrentElement?.Attributes.Markup == "*")
            .Bind(asterismStyle);

        style = pdf.StyleManager.Styles[MarkdownStyleNames.Toc1];
        style.Paragraph.FirstLineIndent = 0;

        style = pdf.StyleManager.AddStyle("frontPage");
        style.Paragraph.Alignment = ParagraphAlignment.Center;
        style.Font.Size = 28;
        style.Font.Bold = true;
        style.Margin.Bottom = "1.5em";
        style.Margin.Top = "1.5em";
        pdf.StyleManager.ForElement(ElementType.Paragraph, "frontPage").Bind(style);
    }
}
