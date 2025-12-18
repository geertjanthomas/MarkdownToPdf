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
        Examples.HelloWorld.Run();
        Examples.HelloWorldWithImage.Run();
        Examples.BasicStyling.Run();
        Examples.CustomStyles.Run();
        Examples.AdvancedStyling.Run();
        Examples.Tables.Run();
        Examples.Sections.Run();
        Examples.Events.Run();
        Examples.Toc.Run();
        Examples.Attributes.Run();
        Examples.FullBook.Run();

        // Out of scope
        //Examples.Features.Run();
        //Examples.Highlighting.Run();
        //Examples.Plugins.Run();
    }
}
