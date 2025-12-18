using VectorAi.MarkdownToPdf;

namespace Test10.Examples;

/// <summary>
/// Demonstration of:
/// use of .Toc styles to render TOC lines with page number
/// </summary>

public static class Toc
{
    public static void Run()
    {
        var filePath = Path.Join(Program.BasePath(),"data/toc.md");
        var markdown = File.ReadAllText(filePath);

        var pdf = new MarkdownToPdf();

        pdf
         .PaperSize(PaperSize.A5)
         .Add(markdown)
         .Save("toc.pdf");
    }
}
