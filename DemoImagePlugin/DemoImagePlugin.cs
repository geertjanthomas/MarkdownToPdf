using CSharpMath.SkiaSharp;
using SkiaSharp;
using System.Drawing;
using System.Drawing.Imaging;
using VectorAi.MarkdownToPdf.Converters;
using VectorAi.MarkdownToPdf.Plugins;

namespace DemoImagePlugin;

public class DemoImagePlugin : IImagePlugin
{
    private string? data;
    private IElementConverter? converter;

    public ImagePluginResult Convert(string data, IElementConverter converter)
    {
        this.data = data;
        this.converter = converter;

        if (converter.Attributes?.Info == "math") return Math();
        return SomeImagePlugin();
    }

    private static ImagePluginResult SomeImagePlugin()
    {
#if DEBUG
        var fileName = Guid.NewGuid().ToString() + ".png";
#else
        var fileName = System.IO.Path.GetTempPath() +   Guid.NewGuid().ToString() + ".png";
#endif

        var w = 100;
        var h = 100;
        using (var bitmap = new Bitmap(w, h))
        {
            bitmap.SetResolution(600, 600);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawLine(Pens.Black, 0, 0, w - 1, h - 1);
                g.DrawLine(Pens.Black, w - 1, 0, 0, h - 1);
            }

            bitmap.Save(fileName, ImageFormat.Png);
        }
        return new ImagePluginResult { FileName = fileName, Success = true };
    }

    private ImagePluginResult Math()
    {
#if DEBUG
        var fileName = Guid.NewGuid().ToString() + ".png";
#else
        var fileName = System.IO.Path.GetTempPath() +   Guid.NewGuid().ToString() + ".png";
#endif



        // Initialize the painter
        var painter = new MathPainter();

        // Set the LaTeX formula
        painter.LaTeX = data;

        // Set visual properties
        painter.FontSize = 20;
        painter.TextColor = SKColors.Black;

        // The DrawAsStream method handles the rendering and encoding to PNG
        using (Stream? stream = painter.DrawAsStream())
        {
            if (stream == null) throw new InvalidOperationException();

            using (var fileStream = File.Create(fileName))
            {
                stream.CopyTo(fileStream);
            }
        }


        return new ImagePluginResult { FileName = fileName, Success = true };
    }
}