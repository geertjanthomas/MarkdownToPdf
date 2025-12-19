// This file is a part of MarkdownToPdf Library by Geert-Jan Thomas based on earlier work by Tomas Kubec
// Distributed under MIT license - see license.txt
//

namespace VectorAi.MarkdownToPdf.MigrDoc;

internal class FontFamily
{
    public string? Folder { get; set; }
    public string Name { get; set; }
    public string Normal { get; set; }
    public string Bold { get; set; }
    public string Italic { get; set; }
    public string BoldItalic { get; set; }

    public FontFamily(string name, string regular, string bold = "", string italic = "", string boldItalic = "", string? folder = null)
    {
        Name = name;
        Normal = regular;
        Bold = !string.IsNullOrEmpty(bold) ? bold : Normal;
        Italic = !string.IsNullOrEmpty(italic) ? italic : Normal;
        BoldItalic = !string.IsNullOrEmpty(boldItalic) ? boldItalic : !string.IsNullOrEmpty(italic) ? italic : regular;
        Folder = folder;
    }
}
