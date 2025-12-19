using VectorAi.MarkdownToPdf;
using VectorAi.MarkdownToPdf.Styling;

namespace Test10.Examples;

/// <summary>
/// Demonstration of:
/// </summary>

public static class Attributes
{
    public static void Run()
    {
        var markdown = File.ReadAllText("../../../data/attributes.md");

        var pdf = new MarkdownToPdf();

        pdf.WarningIssued += (o, e) => { Console.WriteLine($"{e.Category}: {e.Message}"); };

        var fontstyle = pdf.StyleManager.AddStyle("myFont", MarkdownStyleNames.InlineCode);
        fontstyle.Font.Name = "Courier New";
        fontstyle.Font.Bold = true;
        //fontstyle.Font.Color = Color.Red;

        var style = pdf.StyleManager.AddStyle("myList", MarkdownStyleNames.UnorderedListItem);
        style.Bullet.Normal.Content = "*";
        pdf.StyleManager.ForElement(ElementType.UnorderedListItem).WithParent(ElementType.UnorderedList, "myList").Bind(style);
        pdf.StyleManager.ForElement(ElementType.InlineCode).Bind(fontstyle);
        pdf
         .Add(markdown)
         .Save("attributes.pdf");
    }
}
