using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using VectorAi.MarkdownToPdf.Converters;
using VectorAi.MarkdownToPdf.Plugins;

namespace DemoImagePlugin;

public class DemoImagePlugin : IImagePlugin
{
    private string? _data;
    private IElementConverter? _converter;

    public ImagePluginResult Convert(string data, IElementConverter converter)
    {
        _data = data;
        _converter = converter;

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

        // Use SixLabors.ImageSharp.Drawing for cross platform drawing
        using (var image = new Image<Rgba32>(w, h))
        {
            image.Metadata.HorizontalResolution = 600;
            image.Metadata.VerticalResolution = 600;

            image.Mutate(ctx =>
            {
                ctx.DrawLine(Color.Black, 1f, new PointF(0, 0), new PointF(w - 1, h - 1));
                ctx.DrawLine(Color.Black, 1f, new PointF(w - 1, 0), new PointF(0, h - 1));
            });

            image.SaveAsPng(fileName);
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

        // TODO: Find a way to convert LaTeX to a PNG image that is modern and safe

        return new ImagePluginResult { FileName = fileName, Success = true };
    }
}