using VectorAi.MarkdownToPdf;

namespace Test10.Examples;

/// <summary>
/// Demonstration of plugins
/// </summary>

public static class Plugins
{
    public static void Run()
    {
        //throw new NotImplementedException();
        var dp = new DemoImagePlugin.DemoImagePlugin();
        var markdown = File.ReadAllText("../../../data/plugins.md");
        using (var pdf = new MarkdownToPdf())
        {
            pdf.PluginManager.AddMathPlugin(dp);
            pdf.WarningIssued += (o, e) => { Console.WriteLine($"{e.Category}: {e.Message}"); };

            pdf.Add(markdown)

            .Save("plugins.pdf");
        }
    }
}
