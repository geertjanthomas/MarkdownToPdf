using VectorAi.MarkdownToPdf;
using VectorAi.MarkdownToPdf.Styling;
using MigraDoc.DocumentObjectModel;
using System.Runtime.InteropServices;

namespace Test10.Examples;

/// <summary>
/// Demonstration of:
///   - basic page setup
///   - default font
///   - footer with page numbering
/// </summary>

public static class CustomStyles
{
    public static void Run()
    {
        var defaultFont = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Calibri" : "Verdana";

        var filePath = Path.Join(Program.BasePath(),"data/customStyles.md");
        var markdown = File.ReadAllText(filePath);

        var pdf = new MarkdownToPdf();

        // style with modifyied bullet is bound to nested list item (list item with an ancestor - other list litem)
        var style = pdf.StyleManager.AddStyle("NestedListItem", MarkdownStyleNames.UnorderedListItem);
        style.Bullet.Normal.Content = "*";
        pdf.StyleManager.ForElement(ElementType.UnorderedListItem).WithAncestor(ElementType.UnorderedListItem).Bind(style);

        // custom paragraph style "Blue" is created and bound to paragraphs with syle name "blue"
        style = pdf.StyleManager.AddStyle("Blue", MarkdownStyleNames.Paragraph);
        style.Font.Color = Colors.Blue;
        style.Paragraph.Alignment = ParagraphAlignment.Justify;
        pdf.StyleManager.ForElement(ElementType.Paragraph, "blue").Bind(style);

        pdf
         .DefaultFont(defaultFont, 11)
         .Add(markdown)
         .Save("customStyles.pdf");
    }
}
