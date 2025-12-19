using System.Runtime.InteropServices;
using VectorAi.MarkdownToPdf;
using VectorAi.MarkdownToPdf.Styling;

namespace Test10.Examples;

/// <summary>
/// Playground
/// </summary>

public static class Highlighting
{
    public static void Run()
    {
        //throw new NotImplementedException("DemoHighlighter is not yet ported to .NET 10.");
        var filePath = Path.Join(Program.BasePath(), "data/highlighting.md");
        var markdown = File.ReadAllText(filePath);
        var pdf = new MarkdownToPdf();
        pdf.PluginManager.Add(new DemoHighlighter.PythonHighlighter());
        pdf.WarningIssued += (o, e) => { Console.WriteLine($"{e.Category}: {e.Message}"); };

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Default for InlineCode is Consolas but that is not cross platform available
            var inlineCodeFontStyle = pdf.StyleManager.AddStyle("inlineCodeFont", MarkdownStyleNames.InlineCode);
            inlineCodeFontStyle.Font.Name = "Courier New";
            pdf.StyleManager.ForElement(ElementType.InlineCode).Bind(inlineCodeFontStyle);

            var codeFontStyle = pdf.StyleManager.AddStyle("blockCodeFont", MarkdownStyleNames.Code);
            codeFontStyle.Font.Name = "Courier New";
            pdf.StyleManager.ForElement(ElementType.Code).Bind(codeFontStyle);
        }

        pdf.Add(markdown)
            .Save("highlighting.pdf");
    }
}
