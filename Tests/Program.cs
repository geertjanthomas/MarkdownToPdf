using System.Runtime.InteropServices;
using PdfSharp.Fonts;
using VectorAi.MarkdownToPdf.MigrDoc;

namespace Test10;

internal class Program
{
    private static void Main(string[] args)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.WriteLine("Running on Windows - enabling Windows font support");
            GlobalFontSettings.UseWindowsFontsUnderWindows = true;
            GlobalFontSettings.FontResolver = new FontResolver(Environment.GetFolderPath(Environment.SpecialFolder.Fonts));
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Console.WriteLine("Running on Linux - enabling Windows font support under WSL2");
            GlobalFontSettings.UseWindowsFontsUnderWsl2 = true;
        }
        RunAllExamples();
    }

    private static void RunAllExamples()
    {
        RunSafe(Examples.HelloWorld.Run);
        RunSafe(Examples.HelloWorldWithImage.Run);
        RunSafe(Examples.BasicStyling.Run);
        RunSafe(Examples.CustomStyles.Run);
        RunSafe(Examples.AdvancedStyling.Run);
        RunSafe(Examples.Tables.Run);
        RunSafe(Examples.Sections.Run);
        RunSafe(Examples.Events.Run);
        RunSafe(Examples.Toc.Run);
        RunSafe(Examples.Attributes.Run);
        RunSafe(Examples.FullBook.Run);
        RunSafe(Examples.Features.Run);
        RunSafe(Examples.Highlighting.Run);
        //RunSafe(Examples.Plugins.Run);
    }

    private static void RunSafe(Action action)
    {
        try
        {
            Console.WriteLine($"{action.Method.DeclaringType}.{action.Method.Name}");
            action();
        }
        catch(Exception ex)
        {
            var clr = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
            Console.ForegroundColor = clr;
        }
    }

    public static string BasePath()
    {
        var cd = Environment.CurrentDirectory;
        var subs = cd.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries).ToList();
        var idx = subs.FindIndex(s => s == "Tests");
        var basePath = $"{string.Join(Path.DirectorySeparatorChar, subs.Slice(0, idx + 1))}";
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            basePath = $"{Path.DirectorySeparatorChar}{basePath}";
        }
        return basePath;
    }
}
