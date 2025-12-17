using VectorAi.MarkdownToPdf;

namespace Test10.Examples;

/// <summary>
/// Playground
/// </summary>

public static class Highlighting
{
    public static void Run()
    {
        throw new NotImplementedException("DemoHighlighter is not yet ported to .NET 10.");
        //var markdown = File.ReadAllText("../../../data/highlighting.md");
        //var pdf = new MarkdownToPdf();
        //pdf.PluginManager.Add(new DemoHighlighter.Highlighter());
        //pdf.WarningIssued += (o, e) => { Console.WriteLine($"{e.Category}: {e.Message}"); };

        //pdf.Add(markdown)
        //    .Save("highlighting.pdf");
    }
}
