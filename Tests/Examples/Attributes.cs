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
        var fi = WindowsFontFinder.Find("Consolas");
        if (fi != null)
            pdf.RegisterLocalFont(
                fi.Name,
                fi.Regular,
                fi.Bold,
                fi.Italic,
                fi.BoldItalic,
                fi.Folder
                );

        pdf.WarningIssued += (o, e) => { Console.WriteLine($"{e.Category}: {e.Message}"); };

        var style = pdf.StyleManager.AddStyle("myList", MarkdownStyleNames.UnorderedListItem);
        style.Bullet.Normal.Content = "*";
        pdf.StyleManager.ForElement(ElementType.UnorderedListItem).WithParent(ElementType.UnorderedList, "myList").Bind(style);
        pdf
         .Add(markdown)
         .Save("attributes.pdf");
    }
}
