using System.Runtime.InteropServices;
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
        var filePath = Path.Join(Program.BasePath(),"data/attributes.md");
        var imagePath = Path.Join(Program.BasePath(),"data/images");
        var markdown = File.ReadAllText(filePath);

        var pdf = new MarkdownToPdf();

        pdf.WarningIssued += (o, e) => { Console.WriteLine($"{e.Category}: {e.Message}"); };

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Default for InlineCode is Consolas but that is not cross platform available
            var fontstyle = pdf.StyleManager.AddStyle("codeFont", MarkdownStyleNames.InlineCode);
            fontstyle.Font.Name = "Courier New";
            pdf.StyleManager.ForElement(ElementType.InlineCode).Bind(fontstyle);
        }

        var style = pdf.StyleManager.AddStyle("myList", MarkdownStyleNames.UnorderedListItem);
        style.Bullet.Normal.Content = "*";
        pdf.StyleManager.ForElement(ElementType.UnorderedListItem).WithParent(ElementType.UnorderedList, "myList").Bind(style);
        pdf
         .ImageDir(imagePath)
         .Add(markdown)
         .Save("attributes.pdf");
    }
}
