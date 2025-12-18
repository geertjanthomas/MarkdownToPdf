using System.Runtime.InteropServices;
using PdfSharp.Fonts;

namespace Test10;

internal class Program
{
    private static void Main(string[] args)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            GlobalFontSettings.UseWindowsFontsUnderWindows = true;
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            GlobalFontSettings.UseWindowsFontsUnderWsl2 = true;
        }
        RunAllExamples();
    }

    private static void RunAllExamples()
    {
        Examples.HelloWorld.Run();
        //Examples.BasicStyling.Run();
        //Examples.CustomStyles.Run();
        //Examples.AdvancedStyling.Run();
        //Examples.Tables.Run();
        //Examples.Sections.Run();
        //Examples.Events.Run();
        //Examples.Toc.Run();
        //Examples.Highlighting.Run();
        //Examples.Features.Run();
        //Examples.Attributes.Run();
        //Examples.FullBook.Run();

        // Out of scope
        //Examples.Plugins.Run();
    }
}
