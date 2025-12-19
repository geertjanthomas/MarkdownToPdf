using VectorAi.MarkdownToPdf;
using VectorAi.MarkdownToPdf.Styling;
using MigraDoc.DocumentObjectModel;
using System.Runtime.InteropServices;

namespace Test10.Examples;

/// <summary>
/// Demonstration of:
///   - most markup features (without attributes)
/// </summary>

public static class Features
{
    public static void Run()
    {
        //throw new NotImplementedException();
        var imagePath = Path.Join(Program.BasePath(), "data/images");
        var fontPath = Path.Join(Program.BasePath(), "data/fonts");
        var filePath = Path.Join(Program.BasePath(), "data/features.md");
        var markdown = File.ReadAllText(filePath);
        var footer = "{align=center}\r\nPage [](md:page)";
        var pdf = new MarkdownToPdf();

        pdf.PluginManager.Add(new DemoHighlighter.PythonHighlighter());

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Default for InlineCode is Consolas but that is not cross platform available
            var inlineCodeFontStyle = pdf.StyleManager.AddStyle("inlineCodeFont", MarkdownStyleNames.InlineCode);
            inlineCodeFontStyle.Font.Name = "Courier New";
            pdf.StyleManager.ForElement(ElementType.InlineCode).Bind(inlineCodeFontStyle);
            
            var codeFontStyle = pdf.StyleManager.AddStyle("blockCodeFont", MarkdownStyleNames.Code);
            codeFontStyle.Font.Name = "Courier New";
            pdf.StyleManager.ForElement(ElementType.Code).Bind(codeFontStyle);

            pdf.RegisterLocalFont("Wingdings", regular: "wingding.ttf");
        }

        // definition of custom styles used in the document
        var style = pdf.StyleManager.AddStyle("CustomListItem", MarkdownStyleNames.UnorderedListItem);
        style.Bullet.Normal.Content = "\x7B";
        style.Padding.Bottom = "12";
        style.Bullet.Normal.Font.Name = "Wingdings";

        style = pdf.StyleManager.AddStyle("NestedCustomContainer", MarkdownStyleNames.CustomContainer);
        style.Background = Colors.LightSalmon;
        pdf.StyleManager.ForElement(ElementType.CustomContainer).WithParent(ElementType.CustomContainer).Bind(style);

        pdf
         .PaperSize(PaperSize.A4)
         .ImageDir(imagePath)
         .FontDir(fontPath)
         .RegisterLocalFont("Roboto", regular: "Roboto-Light.ttf", bold: "Roboto-Bold.ttf", italic: "Roboto-Italic.ttf")
         .DefaultFont("Roboto", 11)
         .Add(markdown)
         .AddFooter(footer)
         .Save("features.pdf");
    }
}
