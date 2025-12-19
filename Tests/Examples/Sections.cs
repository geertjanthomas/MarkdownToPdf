using System.Runtime.InteropServices;
using VectorAi.MarkdownToPdf;
using VectorAi.MarkdownToPdf.Styling;

namespace Test10.Examples;

/// <summary>
/// Demonstration of:
///   - section breaks
///   - section numbering
/// </summary>

public static class Sections
{
    public static void Run()
    {
        var filePath = Path.Join(Program.BasePath(),"data/sections.md");
        var markdown = File.ReadAllText(filePath);
        var pdf = new MarkdownToPdf();

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Default for InlineCode is Consolas but that is not cross platform available
            var inlineCodeStyle = pdf.StyleManager.AddStyle("inlineCodeFont", MarkdownStyleNames.InlineCode);
            inlineCodeStyle.Font.Name = "Courier New";
            pdf.StyleManager.ForElement(ElementType.InlineCode).Bind(inlineCodeStyle);
        }

        pdf.WarningIssued += (o, e) => { Console.WriteLine($"{e.Category}: {e.Message}"); };

        // by default, section starts with page 42 (just for demonstration purposes)
        pdf.DefaultPageSetup.StartingNumber = 42;

        pdf
            .PageMargins(left: "1cm", right: "1cm", top: "2cm", bottom: "3cm")
            .PaperSize(PaperSize.A5)

            .AddFooter("[](md:page)")
            .Add("# Automatically created first section with no own settings\r\n\r\n")
            .Add(markdown)

            .AddSection()
            .FirstPageNumber(10)
            .PageMargins(left: "6cm", right: "1cm", top: "2cm", bottom: "3cm")
             .Add("# Section with modified margins and page numbering starting from 10\r\n\r\n")
            .Add(markdown)

            .AddSection()
            .Add("# Section with no settings\n\r\n")
            .Add(markdown)

            .AddSection(useDefaultPageSetup: true)
            .PaperSize(PaperSize.A4)
            .Add("# Section with `useDefaultPageSetup` and changed page size\r\n\r\n")
            .Add(markdown)

            .Save("sections.pdf");
    }
}
