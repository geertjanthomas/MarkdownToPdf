using VectorAi.MarkdownToPdf;

namespace Test10.Examples;

public static class HelloWorldWithImage
{
    public static void Run()
    {
        var pdf = new MarkdownToPdf();

        pdf
         .ImageDir("../../../data/images/")
         .Add("## Hello World\r\n\r\nHello!")
         .Add("![Alt text](clock.png)")
         .Save("hello_with_image.pdf");
    }
}
